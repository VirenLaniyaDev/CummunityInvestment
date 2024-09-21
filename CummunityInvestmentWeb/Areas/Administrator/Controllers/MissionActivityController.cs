using CommunityInvestment.DataAccess.Repository.IRepository;
using CommunityInvestment.Models.ViewModels;
using CommunityInvestment.Models;
using Microsoft.AspNetCore.Mvc;
using CommunityInvestment.Application.Common;
using CommunityInvestment.Application.Utilities;
using System.Text.Json.Nodes;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace CummunityInvestmentWeb.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MissionActivityController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IWebHostEnvironment webHostEnvironment;
        public MissionActivityController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            this.webHostEnvironment = webHostEnvironment;
        }


        //-- Managing Missions
        public IActionResult ManageMissions()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetMissionsDataTable()
        {
            DataTableFilterVM missionsDTFilter = new DataTableFilterVM(Request);
            DataTableVM<Mission> missionsDataTable = _unitOfWork.AdminMission.GetMissionsDT(missionsDTFilter);
            return Json(missionsDataTable);
        }

        public IActionResult GetCitiesByCountry(long countryId)
        {
            var Cities = _unitOfWork.Filter.GetCitiesByCountryId(countryId);
            return Json(Cities);
        }

        [Route("Administrator/MissionActivity/AddMission")]
        [Route("Administrator/MissionActivity/EditMission/{missionId}")]
        public IActionResult GetMissionEdit(long? missionId)
        {
            ViewBag.Countries = _unitOfWork.Filter.GetAllCountry();
            ViewBag.Skills = _unitOfWork.Filter.GetAllSkills().Where(ms=>ms.DeletedAt == null && ms.Status == "1");
            ViewBag.Themes = _unitOfWork.Filter.GetAllMissionTheme().Where(theme=>theme.DeletedAt == null && theme.Status == 1);
            AdminMissionVM missionVM = new AdminMissionVM();
            if (missionId != null)
            {
                missionVM = _unitOfWork.AdminMission.GetMissionEdit((long)missionId);
                if (missionVM != null)
                    ViewBag.CountryCities = _unitOfWork.Filter.GetCitiesByCountryId(missionVM.CountryId);
                else
                    BadRequest();
            }
            return View("~/Areas/Administrator/Views/MissionActivity/MissionPage.cshtml", missionVM);
        }

        [HttpPost]
        public IActionResult UploadMissionImages()
        {
            List<IFormFile> MissionImages = Request.Form.Files.ToList();
            List<string> uniqueFileNamesList = new List<string>();
            if (MissionImages != null && MissionImages.Count > 0)
            {
                foreach (IFormFile MissionImage in MissionImages)
                {
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "data", "Images", "Mission");
                    string uniqueFileName = GeneralUtility.UploadFile(uploadsFolder, MissionImage);
                    uniqueFileNamesList.Add(uniqueFileName);
                    //MissionImage.CopyTo(new FileStream(filePath, FileMode.Create));
                }
            };
            return Json(uniqueFileNamesList);
        }
        
        [HttpPost]
        public IActionResult UploadMissionDocuments()
        {
            List<IFormFile> MissionDocuments = Request.Form.Files.ToList();
            List<string> uniqueFileNamesList = new List<string>();
            if (MissionDocuments != null && MissionDocuments.Count > 0)
            {
                foreach (IFormFile MissionImage in MissionDocuments)
                {
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "data", "Documents", "MissionDocs");
                    string uniqueFileName = GeneralUtility.UploadFile(uploadsFolder, MissionImage);
                    uniqueFileNamesList.Add(uniqueFileName);
                }
            };
            return Json(uniqueFileNamesList);
        }

        public IActionResult GetUploadedImages(long missionId)
        {
            List<MissionMedium> missionImages = _unitOfWork.AdminMission.GetExistingMissionImages(missionId);
            List<JsonObject> missionImagesObj = new List<JsonObject>();
            if (missionImages != null && missionImages.Count > 0)
            {
                foreach (var missionImage in missionImages)
                {
                    FileInfo missionImageInfo = GeneralUtility.GetFileInfo(webHostEnvironment.WebRootPath, missionImage.MediaPath);
                    if (missionImageInfo.Exists)
                    {
                        var newObj = new JsonObject
                        {
                            ["missionImageId"] = missionImage.MissionMediaId, 
                            ["name"] = missionImage.MediaName,
                            ["path"] = missionImage.MediaPath,
                            ["size"] = missionImageInfo.Length,
                            ["type"] = "image/"
                        };
                        missionImagesObj.Add(newObj);
                    }
                }
            }
            return Json(missionImagesObj);
        }

        public IActionResult GetUploadedDocuments(long missionId)
        {
            List<MissionDocument> missionDocuments = _unitOfWork.AdminMission.GetExistingMissionDocuments(missionId);
            List<JsonObject> missionImagesObj = new List<JsonObject>();
            if (missionDocuments != null && missionDocuments.Count > 0)
            {
                foreach (var missionDocument in missionDocuments)
                {
                    FileInfo missionDocumentInfo = GeneralUtility.GetFileInfo(webHostEnvironment.WebRootPath, missionDocument.DocumentPath);
                    if (missionDocumentInfo.Exists)
                    {
                        var newObj = new JsonObject
                        {
                            ["missionDocumentId"] = missionDocument.MissionDocumentId,
                            ["name"] = missionDocument.DocumentName,
                            ["path"] = missionDocument.DocumentPath,
                            ["size"] = missionDocumentInfo.Length,
                            ["type"] = missionDocumentInfo.Extension
                        };
                        missionImagesObj.Add(newObj);
                    }
                }
            }
            return Json(missionImagesObj);
        }

        [HttpPost]
        public IActionResult RemoveUploadedImage(long missionImageId)
        {
            MissionMedium missionImage = _unitOfWork.AdminMission.GetMissionImage(missionImageId);
            if (missionImage != null)
            {
                if (_unitOfWork.AdminMission.RemoveMissionImage(missionImage))
                {
                    _unitOfWork.Save();
                    return Ok("File Removed!");
                }
            }
            return BadRequest("File not exists or Some error!");
        }
        
        [HttpPost]
        public IActionResult RemoveUploadedDocument(long missionDocumentId)
        {
            MissionDocument missionDocument = _unitOfWork.AdminMission.GetMissionDocument(missionDocumentId);
            if (missionDocument != null)
            {
                if (_unitOfWork.AdminMission.RemoveMissionDocument(missionDocument))
                {
                    _unitOfWork.Save();
                    return Ok("File Removed!");
                }
            }
            return BadRequest("File not exists or Some error!");
        }

        [HttpPost]
        public async Task<IActionResult> SaveMissionAsync(AdminMissionVM missionData)
        {
            if(missionData.MissionType == "goal")
            {
                ModelState.Remove("StartDate");
                ModelState.Remove("EndDate");
            }
            if (ModelState.IsValid)
            {
                bool IsMissionSaved = await _unitOfWork.AdminMission.SaveMission(missionData);
                if (IsMissionSaved)
                {
                    _unitOfWork.Save();
                    TempData["SuccessMessage"] = "Mission Saved successfully!";
                    return Redirect("/Administrator/MissionActivity/ManageMissions");
                }
                TempData["ErrorMessageTitle"] = ErrorMessages.SomethingWentWrongTitle;
                TempData["ErrorMessage"] = ErrorMessages.SomethingWentWrongMessage;
            }
            return View("~/Areas/Administrator/Views/MissionActivity/MissionPage.cshtml", missionData);
        }

        public IActionResult RemoveMission(long missionId)
        {
            if (_unitOfWork.AdminMission.RemoveMission(missionId))
            {
                _unitOfWork.Save();
                return Ok("Mission Removed Successfully!");
            }
            return BadRequest();
        }

        //-- Manage Mission Themes
        public IActionResult MissionThemes()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetMissionThemesDataTable()
        {
            DataTableFilterVM missionThemesDTFilter = new DataTableFilterVM(Request);
            DataTableVM<MissionTheme> missionThemesDataTable = _unitOfWork.AdminMission.GetMissionThemesDT(missionThemesDTFilter);
            return Json(missionThemesDataTable);
        }

        public IActionResult GetMissionThemeEdit(long missionThemeId)
        {
            var missionTheme = _unitOfWork.Filter.GetMissionThemeById(missionThemeId);
            if(missionTheme == null)
                return NotFound();
            return Json(missionTheme);
        }

        [HttpPost]
        public IActionResult AddOrUpdateMissionTheme(AdminMissionThemeVM missionThemeData)
        {
            string response = _unitOfWork.AdminMission.AddOrUpdateMissionTheme(missionThemeData);
            if(response != null)
            {
                _unitOfWork.Save();
                return Ok(response);
            }
            return BadRequest();
        }

        public IActionResult RemoveMissionTheme(long missionThemeId)
        {
            bool IsMissionThemeRemoved = _unitOfWork.AdminMission.RemoveMissionTheme(missionThemeId);
            if (IsMissionThemeRemoved)
            {
                _unitOfWork.Save();
                return Ok("Mission Theme Removed successfully!");
            }
            return BadRequest();
        }


        //-- Manage Mission Skills
        public IActionResult MissionSkills()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetMissionSkillsDataTable()
        {
            DataTableFilterVM missionSkillsDTFilter = new DataTableFilterVM(Request);
            DataTableVM<Skill> missionSkillsDataTable = _unitOfWork.AdminMission.GetMissionSkillsDT(missionSkillsDTFilter);
            return Json(missionSkillsDataTable);
        }

        public IActionResult GetMissionSkillEdit(long skillId)
        {
            var skill = _unitOfWork.Filter.GetSkillById(skillId);
            if(skill == null)
                return NotFound();
            return Json(skill);
        }

        [HttpPost]
        public IActionResult AddOrUpdateMissionSkill(AdminMissionSkillVM skillData)
        {
            string response = _unitOfWork.AdminMission.AddOrUpdateMissionSkill(skillData);
            if(response != null)
            {
                _unitOfWork.Save();
                return Ok(response);
            }
            return BadRequest();
        }

        public IActionResult RemoveSkill(long skillId)
        {
            bool IsSkillRemoved = _unitOfWork.AdminMission.RemoveSkill(skillId);
            if (IsSkillRemoved)
            {
                _unitOfWork.Save();
                return Ok("Skill Removed successfully!");
            }
            return BadRequest();
        }


        //-- Handle Mission Applications
        public IActionResult MissionApplications()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetMissionApplicationsDataTable()
        {
            DataTableFilterVM missionApplicationsDTFilter = new DataTableFilterVM(Request);
            DataTableVM<AdminMissionApplicationsVM> missionApplicationsDataTable = _unitOfWork.AdminMission.GetMissionApplicationsDT(missionApplicationsDTFilter);
            return Json(missionApplicationsDataTable);
        }

        public IActionResult MissionApplicationAction(long missionApplicationId, string MAaction)
        {
            string MA_ActionResponse = _unitOfWork.AdminMission.MissionApplicationAction(missionApplicationId, MAaction);
            if(MA_ActionResponse != null)
            {
                _unitOfWork.Save();
                return Ok(MA_ActionResponse);
            }
            return BadRequest();
        }
        

        //-- Manage User Stories
        public IActionResult Stories()
        {
            return View();
        }

        public IActionResult GetStoryDetail(long storyId)
        {
            Story story = _unitOfWork.VolunteerStory.GetStoryById(storyId);
            if (story != null)
            {
                story.Description = WebUtility.HtmlDecode(story.Description);
                ViewData["ViewCount"] = _unitOfWork.VolunteerStory.GetStoryViewCounts(storyId);
                return PartialView("~/Areas/Administrator/Views/Shared/_AdminStoryDetail.cshtml", story);
            }
            return BadRequest();
        }

        [HttpPost]
        public IActionResult GetStoriesDataTable()
        {
            DataTableFilterVM storiesDTFilter = new DataTableFilterVM(Request);
            DataTableVM<AdminStoryVM> storiesDataTable = _unitOfWork.AdminMission.GetStoriesDT(storiesDTFilter);
            return Json(storiesDataTable);
        }

        public IActionResult StoryAction(long storyId, string storyAction)
        {
            string Story_ActionResponse = _unitOfWork.AdminMission.StoryAction(storyId, storyAction);
            if(Story_ActionResponse != null)
            {
                _unitOfWork.Save();
                return Ok(Story_ActionResponse);
            }
            return BadRequest();
        }

        public IActionResult RemoveStory(long storyId)
        {
            bool IsStoryRemoved = _unitOfWork.AdminMission.RemoveStory(storyId);
            if (IsStoryRemoved)
            {
                _unitOfWork.Save();
                return Ok("Story Removed successfully!");
            }
            return BadRequest();
        }
    }
}
