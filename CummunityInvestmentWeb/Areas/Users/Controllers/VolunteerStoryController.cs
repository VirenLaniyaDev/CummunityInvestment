using CommunityInvestment.Application.Utilities;
using CommunityInvestment.DataAccess.Repository;
using CommunityInvestment.DataAccess.Repository.IRepository;
using CommunityInvestment.Models;
using CommunityInvestment.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol.Core.Types;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace CummunityInvestmentWeb.Users.Controllers
{
    [Authorize(Roles = "User")]
    public class VolunteerStoryController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IWebHostEnvironment webHostEnvironment;
        public VolunteerStoryController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {

            int pg = 1;
            const int pageSize = 9;

            List<Story> Stories = _unitOfWork.VolunteerStory.GetAllStories();
            // Pagination Code
            int storyCounts = Stories.Count();
            if (pg < 1)
                pg = 1;
            int totalPages = Pager.getTotalPages(storyCounts, pageSize);
            if (pg > totalPages)
                pg = totalPages;
            int recSkip = (pg - 1) * pageSize;
            var pager = new Pager(storyCounts, pg, pageSize);
            // Stories on Current page
            Stories = Stories.Skip(recSkip).Take(pager.PageSize).ToList();
            Stories.ForEach(s => s.Description = (s.Description != null) ? Regex.Replace(WebUtility.HtmlDecode(s.Description), "<.*?>", string.Empty) : null);
            Stories.ForEach(s => s.Description = (s.Description != null) ? Regex.Replace(WebUtility.HtmlDecode(s.Description), "&nbsp;", " ") : null);

            VolunteerStoryListingVM StoryListing = new VolunteerStoryListingVM
            {
                Pager = pager,
                Stories = Stories
            };
            return View(StoryListing);
        }

        public IActionResult GetStories(int pg)
        {
            const int pageSize = 9;
            List<Story> Stories = _unitOfWork.VolunteerStory.GetAllStories();
            // Pagination Code
            int storyCounts = Stories.Count();
            if (pg < 1)
                pg = 1;
            int totalPages = Pager.getTotalPages(storyCounts, pageSize);
            if (pg > totalPages)
                pg = totalPages;
            int recSkip = (pg - 1) * pageSize;
            var pager = new Pager(storyCounts, pg, pageSize);
            // Stories on Current page
            Stories = Stories.Skip(recSkip).Take(pager.PageSize).ToList();
            Stories.ForEach(s => s.Description = (s.Description != null) ? Regex.Replace(WebUtility.HtmlDecode(s.Description), "<.*?>", string.Empty) : null);
            Stories.ForEach(s => s.Description = (s.Description != null) ? Regex.Replace(WebUtility.HtmlDecode(s.Description), "&nbsp;", " ") : null);

            VolunteerStoryListingVM StoryListing = new VolunteerStoryListingVM
            {
                Pager = pager,
                Stories = Stories
            };

            return PartialView("~/Areas/Users/Views/Shared/_VolunteerStoryListing.cshtml", StoryListing);
        }

        //[Authorize]
        [Route("Users/VolunteerStory/Story/{storyId}")]
        public IActionResult StoryDetail(long storyId)
        {
            long userId = GeneralUtility.GetClaimIdentifier(User);
            Story story = _unitOfWork.VolunteerStory.GetStoryById(storyId);
            if (story != null)
            {
                story.Description = WebUtility.HtmlDecode(story.Description);
                _unitOfWork.VolunteerStory.IncrementStoryViewCount(storyId, userId);
                _unitOfWork.Save();
                ViewData["ViewCount"] = _unitOfWork.VolunteerStory.GetStoryViewCounts(storyId);
                return View("~/Areas/Users/Views/VolunteerStory/StoryDetail.cshtml", story);
            }
            return BadRequest();
        }

        //[Authorize]
        public IActionResult ShareStory()
        {
            if (User.FindFirst(ClaimTypes.NameIdentifier) != null)
            {
                long userId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                List<MissionApplication> UserVolunteeredMA = _unitOfWork.VolunteerStory.GetUserVolunteeredMA(userId);
                ViewBag.UserVolunteeredMA = UserVolunteeredMA;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShareStory(StoryCreateVM storyCreateObj)
        {
            storyCreateObj.Description = WebUtility.HtmlEncode(storyCreateObj.Description);
            if (User.FindFirst(ClaimTypes.NameIdentifier) != null)
            {
                long userId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                List<MissionApplication> UserVolunteeredMA = _unitOfWork.VolunteerStory.GetUserVolunteeredMA(userId);
                ViewBag.UserVolunteeredMA = UserVolunteeredMA;
            }
            if (ModelState.IsValid)
            {
                bool IsStoryCreated = await _unitOfWork.VolunteerStory.CreateStory(storyCreateObj);
                if (IsStoryCreated)
                {
                    _unitOfWork.Save();
                    return RedirectToAction("UserStories", "VolunteerStory");
                }
            }
            return View(storyCreateObj);
        }

        [HttpPost]
        [Route("Users/VolunteerStory/UploadPhotos")]
        public IActionResult UploadPhotos()
        {
            List<IFormFile> StoryImages = Request.Form.Files.ToList();
            List<string> uniqueFileNamesList = new List<string>();
            if (StoryImages != null && StoryImages.Count > 0)
            {
                foreach (IFormFile StoryImage in StoryImages)
                {
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "data", "Images", "Story");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + StoryImage.FileName;
                    uniqueFileNamesList.Add(uniqueFileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    StoryImage.CopyTo(new FileStream(filePath, FileMode.Create));
                }
            };
            return Json(uniqueFileNamesList);
        }

        [HttpPost]
        [Route("Users/VolunteerStory/RemoveUploadedImage")]
        public IActionResult RemoveUploadedImage(long storyId, string fileName)
        {
            StoryMedium storyImage = _unitOfWork.VolunteerStory.GetStoryImage(storyId, fileName);
            if (storyImage != null)
            {
                _unitOfWork.VolunteerStory.RemoveStoryImage(storyImage);
                _unitOfWork.Save();
                //FileInfo storyImageInfo = GeneralUtility.GetFileInfo(webHostEnvironment.WebRootPath, storyImage.StoryPath);

                //if (storyImageInfo.Exists)
                //{
                //    storyImageInfo.Delete();
                //    return Ok("File removed!");
                //}
                return Ok("File removed!");
            }
            return Ok("File not exists!");
        }

        public IActionResult GetUploadedImages(long storyId)
        {
            List<StoryMedium> storyImages = _unitOfWork.VolunteerStory.GetExistingImages(storyId);
            List<JsonObject> storyImagesObj = new List<JsonObject>();
            if (storyImages != null && storyImages.Count > 0)
            {
                foreach (var storyImage in storyImages)
                {
                    FileInfo storyImageInfo = GeneralUtility.GetFileInfo(webHostEnvironment.WebRootPath, storyImage.StoryPath);
                    if (storyImageInfo.Exists)
                    {
                        var newObj = new JsonObject
                        {
                            ["name"] = storyImageInfo.Name,
                            ["size"] = storyImageInfo.Length,
                            ["type"] = "image/"
                        };
                        storyImagesObj.Add(newObj);
                    }
                }
            }
            return Json(storyImagesObj);
        }

        //[Authorize]
        //[Route("Users/VolunteerStory/UserStories")]
        public IActionResult UserStories()
        {
            //if (User.FindFirst(ClaimTypes.NameIdentifier) != null)
            //{
            //    long userId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            //    List<Story> userStories = _unitOfWork.VolunteerStory.GetUserStories(userId);
            //    return View("~/Areas/Users/Views/VolunteerStory/UserStories.cshtml", userStories);
            //}
            //return BadRequest();
            return View("~/Areas/Users/Views/VolunteerStory/UserStories.cshtml");

        }

        //[Authorize]
        [HttpPost]
        [Route("Users/VolunteerStory/GetUserStories")]
        public IActionResult GetUserStories(string SearchStory, string StoryStatus, string StorySortBy, int pg = 1)
        {
            var filters = new Dictionary<string, string>
            {
                { "SearchString", SearchStory },
                { "StoryStatus", StoryStatus },
                { "StorySortBy", StorySortBy }
            };
            if (User.FindFirst(ClaimTypes.NameIdentifier) != null)
            {
                long userId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                PageList<Story> userStoriesPage = _unitOfWork.VolunteerStory.GetUserStoriesPage(userId, pg, 10, filters);
                return PartialView("~/Areas/Users/Views/Shared/_UserStoriesTable.cshtml", userStoriesPage);
            }
            return BadRequest();
        }

        //[Authorize]
        [Route("Users/VolunteerStory/Edit/{storyId}")]
        public IActionResult GetUserStory(long storyId)
        {
            if (User.FindFirst(ClaimTypes.NameIdentifier) != null)
            {
                long userId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                List<MissionApplication> UserVolunteeredMA = _unitOfWork.VolunteerStory.GetUserVolunteeredMA(userId);
                ViewBag.UserVolunteeredMA = UserVolunteeredMA;

                Story? story = _unitOfWork.VolunteerStory.GetUserStory(storyId, userId);
                if (story != null)
                {
                    List<StoryMedium> storyVideosList = _unitOfWork.VolunteerStory.GetExistingVideos(storyId);
                    List<string> storyVideosURLsList = storyVideosList.Select(sv => sv.StoryPath).ToList();
                    string storyVideoUrls = "";
                    if (storyVideosList != null && storyVideosList.Count > 0)
                        storyVideoUrls = string.Join(Environment.NewLine, storyVideosURLsList);
                    StoryCreateVM storyCreateVM = new StoryCreateVM
                    {
                        StoryId = story.StoryId,
                        MissionId = story.MissionId,
                        UserId = story.UserId,
                        Title = story.Title,
                        Description = WebUtility.HtmlDecode(story.Description),
                        StoryVideoURLs = storyVideoUrls,
                        StoryStatus = story.Status,
                        PublishedAt = story.PublishedAt
                    };
                    return View("~/Areas/Users/Views/VolunteerStory/ShareStory.cshtml", storyCreateVM);
                }
            }
            return BadRequest();
        }

        [Route("Users/VolunteerStory/GetCoWorkers")]
        public IActionResult GetCoWorkers(string searchFilter)
        {
            if (User.FindFirst(ClaimTypes.NameIdentifier) != null)
            {
                long userId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                List<User> coWorkers = _unitOfWork.User.GetCoWorkers(userId, searchFilter);
                return PartialView("~/Areas/Users/Views/Shared/_RecommendStoryToCoWorkers.cshtml", coWorkers);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("Users/VolunteerStory/RecommendToCoWorker")]
        public IActionResult RecommendToCoWorker(long storyId, long coWorkerId)
        {
            if (User.FindFirst(ClaimTypes.NameIdentifier) != null)
            {
                long userId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                _unitOfWork.VolunteerStory.RecommendToCoWorker(storyId, userId, coWorkerId);
                _unitOfWork.Save();
                return Ok("Mail Sent");
            }
            return BadRequest();
        }

        [Route("Users/VolunteerStory/Preview/{storyId}")]
        public IActionResult UserStoryPreview(long storyId)
        {
            if (User.FindFirst(ClaimTypes.NameIdentifier) != null)
            {
                long userId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                Story userStory = _unitOfWork.VolunteerStory.GetUserPreviewStory(userId, storyId);
                if (userStory != null)
                {
                    userStory.Description = WebUtility.HtmlDecode(userStory.Description);
                    _unitOfWork.VolunteerStory.IncrementStoryViewCount(storyId, userId);
                    _unitOfWork.Save();
                    ViewData["ViewCount"] = _unitOfWork.VolunteerStory.GetStoryViewCounts(storyId);
                    return View("~/Areas/Users/Views/VolunteerStory/StoryDetail.cshtml", userStory);
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("Users/VolunteerStory/RemoveUserStory")]
        public IActionResult RemoveUserStory(long StoryId)
        {
            if (User.FindFirst(ClaimTypes.NameIdentifier) != null)
            {
                long userId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                //Story userStory = _unitOfWork.VolunteerStory.GetUserStory(StoryId, userId);
                //if (userStory != null)
                //{
                //    //return RedirectToAction("UserStories", "VolunteerStory");
                //}
                try
                {
                    Story removedStory = _unitOfWork.VolunteerStory.RemoveStory(StoryId, userId);
                    _unitOfWork.Save();
                    return RedirectToAction("UserStories", "VolunteerStory");
                }
                catch
                {
                    return BadRequest();
                }
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("Users/VolunteerStory/RemoveUserStoryAjax")]
        public IActionResult RemoveUserStoryAjax(long StoryId, int pg = 1)
        {
            if (User.FindFirst(ClaimTypes.NameIdentifier) != null)
            {
                long userId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                try
                {
                    Story removedStory = _unitOfWork.VolunteerStory.RemoveStory(StoryId, userId);
                    _unitOfWork.Save();
                    PageList<Story> userStoriesPage = _unitOfWork.VolunteerStory.GetUserStoriesPage(userId, pg, 10);
                    return PartialView("~/Areas/Users/Views/Shared/_UserStoriesTable.cshtml", userStoriesPage);
                }
                catch
                {
                    return BadRequest();
                }
            }
            return BadRequest();
        }
    }
}
