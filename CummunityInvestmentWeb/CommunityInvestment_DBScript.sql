USE [master]
GO
/****** Object:  Database [CommunityInvestment]    Script Date: 21-09-2024 23:08:27 ******/
CREATE DATABASE [CommunityInvestment]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CommunityInvestment', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\CommunityInvestment.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'CommunityInvestment_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\CommunityInvestment_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [CommunityInvestment] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CommunityInvestment].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CommunityInvestment] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CommunityInvestment] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CommunityInvestment] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CommunityInvestment] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CommunityInvestment] SET ARITHABORT OFF 
GO
ALTER DATABASE [CommunityInvestment] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [CommunityInvestment] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CommunityInvestment] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CommunityInvestment] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CommunityInvestment] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CommunityInvestment] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CommunityInvestment] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CommunityInvestment] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CommunityInvestment] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CommunityInvestment] SET  DISABLE_BROKER 
GO
ALTER DATABASE [CommunityInvestment] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CommunityInvestment] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CommunityInvestment] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CommunityInvestment] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CommunityInvestment] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CommunityInvestment] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [CommunityInvestment] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CommunityInvestment] SET RECOVERY FULL 
GO
ALTER DATABASE [CommunityInvestment] SET  MULTI_USER 
GO
ALTER DATABASE [CommunityInvestment] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CommunityInvestment] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CommunityInvestment] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CommunityInvestment] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [CommunityInvestment] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [CommunityInvestment] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'CommunityInvestment', N'ON'
GO
ALTER DATABASE [CommunityInvestment] SET QUERY_STORE = ON
GO
ALTER DATABASE [CommunityInvestment] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [CommunityInvestment]
GO
/****** Object:  Table [dbo].[admin]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[admin](
	[admin_id] [bigint] IDENTITY(1,1) NOT NULL,
	[first_name] [varchar](16) NULL,
	[last_name] [varchar](16) NULL,
	[email] [varchar](128) NOT NULL,
	[password] [varchar](255) NOT NULL,
	[avatar] [varchar](2048) NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[admin_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[banner]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[banner](
	[banner_id] [bigint] IDENTITY(1,1) NOT NULL,
	[image] [varchar](512) NOT NULL,
	[title] [varchar](160) NOT NULL,
	[text] [text] NULL,
	[sort_order] [int] NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[banner_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[city]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[city](
	[city_id] [bigint] IDENTITY(1,1) NOT NULL,
	[country_id] [bigint] NOT NULL,
	[name] [varchar](255) NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[city_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[cms_page]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cms_page](
	[cms_page_id] [bigint] IDENTITY(1,1) NOT NULL,
	[title] [varchar](255) NULL,
	[description] [text] NULL,
	[slug] [varchar](255) NOT NULL,
	[status] [varchar](20) NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[cms_page_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[comment]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[comment](
	[comment_id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_id] [bigint] NULL,
	[mission_id] [bigint] NOT NULL,
	[comment_text] [text] NOT NULL,
	[approval_status] [varchar](20) NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[comment_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[contact_us]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[contact_us](
	[contact_id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_id] [bigint] NOT NULL,
	[subject] [nvarchar](255) NULL,
	[message] [text] NULL,
	[status] [varchar](20) NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[contact_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[country]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[country](
	[country_id] [bigint] IDENTITY(1,1) NOT NULL,
	[name] [varchar](255) NOT NULL,
	[iso] [varchar](16) NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[country_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[favorite_mission]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[favorite_mission](
	[favourite_mission_id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_id] [bigint] NULL,
	[mission_id] [bigint] NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[favourite_mission_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[goal_mission]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[goal_mission](
	[goal_mission_id] [bigint] IDENTITY(1,1) NOT NULL,
	[mission_id] [bigint] NOT NULL,
	[goal_objective_text] [varchar](255) NULL,
	[goal_value] [int] NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[goal_mission_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mission]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mission](
	[mission_id] [bigint] IDENTITY(1,1) NOT NULL,
	[city_id] [bigint] NOT NULL,
	[country_id] [bigint] NOT NULL,
	[theme_id] [bigint] NOT NULL,
	[title] [varchar](128) NOT NULL,
	[short_description] [text] NULL,
	[description] [text] NULL,
	[start_date] [datetime] NULL,
	[end_date] [datetime] NULL,
	[registration_deadline] [datetime] NULL,
	[mission_type] [varchar](20) NOT NULL,
	[status] [varchar](20) NULL,
	[organization_name] [varchar](255) NULL,
	[organization_detail] [text] NULL,
	[availability] [varchar](20) NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[mission_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mission_application]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mission_application](
	[mission_application_id] [bigint] IDENTITY(1,1) NOT NULL,
	[mission_id] [bigint] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[applied_at] [datetime] NOT NULL,
	[approval_status] [varchar](20) NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[mission_application_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mission_document]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mission_document](
	[mission_document_id] [bigint] IDENTITY(1,1) NOT NULL,
	[mission_id] [bigint] NOT NULL,
	[document_name] [varchar](255) NOT NULL,
	[document_type] [varchar](255) NOT NULL,
	[document_path] [varchar](255) NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[mission_document_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mission_invite]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mission_invite](
	[mission_invite_id] [bigint] IDENTITY(1,1) NOT NULL,
	[mission_id] [bigint] NOT NULL,
	[from_user_id] [bigint] NOT NULL,
	[to_user_id] [bigint] NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[mission_invite_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mission_media]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mission_media](
	[mission_media_id] [bigint] IDENTITY(1,1) NOT NULL,
	[mission_id] [bigint] NOT NULL,
	[media_name] [nvarchar](255) NULL,
	[media_type] [nvarchar](255) NULL,
	[media_path] [nvarchar](255) NULL,
	[default_] [nvarchar](20) NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[mission_media_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mission_rating]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mission_rating](
	[mission_rating_id] [bigint] IDENTITY(1,1) NOT NULL,
	[mission_id] [bigint] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[rating] [nvarchar](20) NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[mission_rating_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mission_skill]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mission_skill](
	[mission_skill_id] [bigint] IDENTITY(1,1) NOT NULL,
	[skill_id] [bigint] NOT NULL,
	[mission_id] [bigint] NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[mission_skill_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mission_theme]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mission_theme](
	[mission_theme_id] [bigint] IDENTITY(1,1) NOT NULL,
	[title] [nvarchar](255) NULL,
	[status] [tinyint] NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[mission_theme_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[notification_settings]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[notification_settings](
	[notification_setting_id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_id] [bigint] NOT NULL,
	[recommended_missions] [tinyint] NULL,
	[volunteering_hours] [tinyint] NULL,
	[volunteering_goals] [tinyint] NULL,
	[user_comments] [tinyint] NULL,
	[user_stories] [tinyint] NULL,
	[new_missions] [tinyint] NULL,
	[new_messages] [tinyint] NULL,
	[recommended_story] [tinyint] NULL,
	[notification_by_email] [tinyint] NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
 CONSTRAINT [PK_notification_settings] PRIMARY KEY CLUSTERED 
(
	[notification_setting_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[password_reset]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[password_reset](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[email] [nvarchar](191) NULL,
	[token] [nvarchar](191) NULL,
	[created_at] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[skill]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[skill](
	[skill_id] [bigint] IDENTITY(1,1) NOT NULL,
	[skill_name] [nvarchar](64) NULL,
	[status] [nvarchar](1) NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[skill_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[story]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[story](
	[story_id] [bigint] IDENTITY(1,1) NOT NULL,
	[mission_id] [bigint] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[title] [varchar](255) NULL,
	[description] [text] NULL,
	[status] [varchar](20) NULL,
	[published_at] [datetime] NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[story_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[story_invite]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[story_invite](
	[story_invite_id] [bigint] IDENTITY(1,1) NOT NULL,
	[story_id] [bigint] NOT NULL,
	[from_user_id] [bigint] NOT NULL,
	[to_user_id] [bigint] NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[story_invite_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[story_media]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[story_media](
	[story_media_id] [bigint] IDENTITY(1,1) NOT NULL,
	[story_id] [bigint] NOT NULL,
	[story_type] [varchar](8) NOT NULL,
	[story_path] [nvarchar](max) NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[story_media_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[story_view_count]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[story_view_count](
	[story_view_count_id] [bigint] IDENTITY(1,1) NOT NULL,
	[story_id] [bigint] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
 CONSTRAINT [PK_story_view_count] PRIMARY KEY CLUSTERED 
(
	[story_view_count_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[timesheet]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[timesheet](
	[timesheet_id] [bigint] IDENTITY(1,1) NOT NULL,
	[mission_id] [bigint] NULL,
	[user_id] [bigint] NULL,
	[timesheet_time] [time](7) NULL,
	[action] [int] NULL,
	[date_volunteered] [datetime] NOT NULL,
	[notes] [text] NULL,
	[status] [varchar](20) NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[timesheet_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user_notification]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_notification](
	[user_notification_id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_id] [bigint] NULL,
	[notification_for] [varchar](50) NOT NULL,
	[notification_message] [nvarchar](max) NULL,
	[notification_link] [nvarchar](1024) NULL,
	[is_read] [tinyint] NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[user_notification_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[user_skill]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[user_skill](
	[user_skill_id] [bigint] IDENTITY(1,1) NOT NULL,
	[skill_id] [bigint] NOT NULL,
	[user_id] [bigint] NOT NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[user_skill_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[user_id] [bigint] IDENTITY(1,1) NOT NULL,
	[first_name] [varchar](16) NULL,
	[last_name] [varchar](16) NULL,
	[email] [varchar](128) NOT NULL,
	[password] [varchar](255) NOT NULL,
	[phone_number] [int] NOT NULL,
	[avatar] [varchar](2048) NULL,
	[why_i_volunteer] [text] NULL,
	[employee_id] [varchar](16) NULL,
	[department] [varchar](16) NULL,
	[city_id] [bigint] NOT NULL,
	[country_id] [bigint] NOT NULL,
	[profile_text] [text] NULL,
	[linked_in_url] [varchar](255) NULL,
	[title] [varchar](255) NULL,
	[status] [varchar](20) NOT NULL,
	[availability] [varchar](20) NULL,
	[created_at] [datetime] NOT NULL,
	[updated_at] [datetime] NULL,
	[deleted_at] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[admin] ADD  DEFAULT (NULL) FOR [first_name]
GO
ALTER TABLE [dbo].[admin] ADD  DEFAULT (NULL) FOR [last_name]
GO
ALTER TABLE [dbo].[admin] ADD  DEFAULT (NULL) FOR [avatar]
GO
ALTER TABLE [dbo].[admin] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[admin] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[admin] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[banner] ADD  DEFAULT ((0)) FOR [sort_order]
GO
ALTER TABLE [dbo].[banner] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[banner] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[banner] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[city] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[city] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[city] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[cms_page] ADD  DEFAULT ((1)) FOR [status]
GO
ALTER TABLE [dbo].[cms_page] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[cms_page] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[cms_page] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[comment] ADD  DEFAULT ('pending') FOR [approval_status]
GO
ALTER TABLE [dbo].[comment] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[comment] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[comment] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[contact_us] ADD  DEFAULT (NULL) FOR [subject]
GO
ALTER TABLE [dbo].[contact_us] ADD  DEFAULT (NULL) FOR [message]
GO
ALTER TABLE [dbo].[contact_us] ADD  DEFAULT ('pending') FOR [status]
GO
ALTER TABLE [dbo].[contact_us] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[contact_us] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[contact_us] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[country] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[country] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[country] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[favorite_mission] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[favorite_mission] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[favorite_mission] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[goal_mission] ADD  DEFAULT (NULL) FOR [goal_objective_text]
GO
ALTER TABLE [dbo].[goal_mission] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[goal_mission] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[goal_mission] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[mission] ADD  DEFAULT (NULL) FOR [description]
GO
ALTER TABLE [dbo].[mission] ADD  DEFAULT (NULL) FOR [start_date]
GO
ALTER TABLE [dbo].[mission] ADD  DEFAULT (NULL) FOR [end_date]
GO
ALTER TABLE [dbo].[mission] ADD  DEFAULT (NULL) FOR [registration_deadline]
GO
ALTER TABLE [dbo].[mission] ADD  DEFAULT (NULL) FOR [organization_name]
GO
ALTER TABLE [dbo].[mission] ADD  DEFAULT (NULL) FOR [organization_detail]
GO
ALTER TABLE [dbo].[mission] ADD  DEFAULT (NULL) FOR [availability]
GO
ALTER TABLE [dbo].[mission] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[mission] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[mission] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[mission_application] ADD  DEFAULT ('pending') FOR [approval_status]
GO
ALTER TABLE [dbo].[mission_application] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[mission_application] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[mission_application] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[mission_document] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[mission_document] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[mission_document] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[mission_invite] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[mission_invite] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[mission_invite] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[mission_media] ADD  DEFAULT ((0)) FOR [default_]
GO
ALTER TABLE [dbo].[mission_media] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[mission_media] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[mission_media] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[mission_rating] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[mission_rating] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[mission_rating] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[mission_skill] ADD  DEFAULT (NULL) FOR [mission_id]
GO
ALTER TABLE [dbo].[mission_skill] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[mission_skill] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[mission_skill] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[mission_theme] ADD  DEFAULT ((1)) FOR [status]
GO
ALTER TABLE [dbo].[mission_theme] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[mission_theme] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[mission_theme] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[notification_settings] ADD  DEFAULT ((1)) FOR [recommended_missions]
GO
ALTER TABLE [dbo].[notification_settings] ADD  DEFAULT ((0)) FOR [volunteering_hours]
GO
ALTER TABLE [dbo].[notification_settings] ADD  DEFAULT ((0)) FOR [volunteering_goals]
GO
ALTER TABLE [dbo].[notification_settings] ADD  DEFAULT ((0)) FOR [user_comments]
GO
ALTER TABLE [dbo].[notification_settings] ADD  DEFAULT ((1)) FOR [user_stories]
GO
ALTER TABLE [dbo].[notification_settings] ADD  DEFAULT ((1)) FOR [new_missions]
GO
ALTER TABLE [dbo].[notification_settings] ADD  DEFAULT ((0)) FOR [new_messages]
GO
ALTER TABLE [dbo].[notification_settings] ADD  DEFAULT ((1)) FOR [recommended_story]
GO
ALTER TABLE [dbo].[notification_settings] ADD  DEFAULT ((0)) FOR [notification_by_email]
GO
ALTER TABLE [dbo].[notification_settings] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[notification_settings] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[notification_settings] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[password_reset] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[skill] ADD  DEFAULT ((1)) FOR [status]
GO
ALTER TABLE [dbo].[skill] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[skill] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[skill] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[story] ADD  DEFAULT (NULL) FOR [title]
GO
ALTER TABLE [dbo].[story] ADD  DEFAULT (NULL) FOR [description]
GO
ALTER TABLE [dbo].[story] ADD  DEFAULT ('draft') FOR [status]
GO
ALTER TABLE [dbo].[story] ADD  DEFAULT (NULL) FOR [published_at]
GO
ALTER TABLE [dbo].[story] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[story] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[story] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[story_invite] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[story_invite] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[story_invite] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[story_media] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[story_media] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[story_media] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[story_view_count] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[story_view_count] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[story_view_count] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[timesheet] ADD  DEFAULT ('pending') FOR [status]
GO
ALTER TABLE [dbo].[timesheet] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[timesheet] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[timesheet] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[user_notification] ADD  DEFAULT (NULL) FOR [user_id]
GO
ALTER TABLE [dbo].[user_notification] ADD  DEFAULT (NULL) FOR [notification_link]
GO
ALTER TABLE [dbo].[user_notification] ADD  DEFAULT ((0)) FOR [is_read]
GO
ALTER TABLE [dbo].[user_notification] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[user_notification] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[user_notification] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[user_skill] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[user_skill] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[user_skill] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (NULL) FOR [first_name]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (NULL) FOR [last_name]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (NULL) FOR [avatar]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (NULL) FOR [why_i_volunteer]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (NULL) FOR [employee_id]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (NULL) FOR [department]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (NULL) FOR [profile_text]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (NULL) FOR [linked_in_url]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (NULL) FOR [title]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT ((1)) FOR [status]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (NULL) FOR [availability]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (getdate()) FOR [created_at]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (NULL) FOR [updated_at]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (NULL) FOR [deleted_at]
GO
ALTER TABLE [dbo].[city]  WITH CHECK ADD FOREIGN KEY([country_id])
REFERENCES [dbo].[country] ([country_id])
GO
ALTER TABLE [dbo].[comment]  WITH CHECK ADD FOREIGN KEY([mission_id])
REFERENCES [dbo].[mission] ([mission_id])
GO
ALTER TABLE [dbo].[comment]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[contact_us]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[favorite_mission]  WITH CHECK ADD FOREIGN KEY([mission_id])
REFERENCES [dbo].[mission] ([mission_id])
GO
ALTER TABLE [dbo].[favorite_mission]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[goal_mission]  WITH CHECK ADD FOREIGN KEY([mission_id])
REFERENCES [dbo].[mission] ([mission_id])
GO
ALTER TABLE [dbo].[mission]  WITH CHECK ADD FOREIGN KEY([city_id])
REFERENCES [dbo].[city] ([city_id])
GO
ALTER TABLE [dbo].[mission]  WITH CHECK ADD FOREIGN KEY([country_id])
REFERENCES [dbo].[country] ([country_id])
GO
ALTER TABLE [dbo].[mission]  WITH CHECK ADD FOREIGN KEY([theme_id])
REFERENCES [dbo].[mission_theme] ([mission_theme_id])
GO
ALTER TABLE [dbo].[mission_application]  WITH CHECK ADD FOREIGN KEY([mission_id])
REFERENCES [dbo].[mission] ([mission_id])
GO
ALTER TABLE [dbo].[mission_application]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[mission_document]  WITH CHECK ADD FOREIGN KEY([mission_id])
REFERENCES [dbo].[mission] ([mission_id])
GO
ALTER TABLE [dbo].[mission_invite]  WITH CHECK ADD FOREIGN KEY([from_user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[mission_invite]  WITH CHECK ADD FOREIGN KEY([mission_id])
REFERENCES [dbo].[mission] ([mission_id])
GO
ALTER TABLE [dbo].[mission_invite]  WITH CHECK ADD FOREIGN KEY([to_user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[mission_media]  WITH CHECK ADD FOREIGN KEY([mission_id])
REFERENCES [dbo].[mission] ([mission_id])
GO
ALTER TABLE [dbo].[mission_rating]  WITH CHECK ADD FOREIGN KEY([mission_id])
REFERENCES [dbo].[mission] ([mission_id])
GO
ALTER TABLE [dbo].[mission_rating]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[mission_skill]  WITH CHECK ADD FOREIGN KEY([mission_id])
REFERENCES [dbo].[mission] ([mission_id])
GO
ALTER TABLE [dbo].[mission_skill]  WITH CHECK ADD FOREIGN KEY([skill_id])
REFERENCES [dbo].[skill] ([skill_id])
GO
ALTER TABLE [dbo].[notification_settings]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[story]  WITH CHECK ADD FOREIGN KEY([mission_id])
REFERENCES [dbo].[mission] ([mission_id])
GO
ALTER TABLE [dbo].[story]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[story_invite]  WITH CHECK ADD FOREIGN KEY([from_user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[story_invite]  WITH CHECK ADD FOREIGN KEY([story_id])
REFERENCES [dbo].[story] ([story_id])
GO
ALTER TABLE [dbo].[story_invite]  WITH CHECK ADD FOREIGN KEY([to_user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[story_media]  WITH CHECK ADD FOREIGN KEY([story_id])
REFERENCES [dbo].[story] ([story_id])
GO
ALTER TABLE [dbo].[story_view_count]  WITH CHECK ADD FOREIGN KEY([story_id])
REFERENCES [dbo].[story] ([story_id])
GO
ALTER TABLE [dbo].[story_view_count]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[timesheet]  WITH CHECK ADD FOREIGN KEY([mission_id])
REFERENCES [dbo].[mission] ([mission_id])
GO
ALTER TABLE [dbo].[timesheet]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[user_notification]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[user_skill]  WITH CHECK ADD FOREIGN KEY([skill_id])
REFERENCES [dbo].[skill] ([skill_id])
GO
ALTER TABLE [dbo].[user_skill]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([user_id])
GO
ALTER TABLE [dbo].[users]  WITH CHECK ADD FOREIGN KEY([city_id])
REFERENCES [dbo].[city] ([city_id])
GO
ALTER TABLE [dbo].[users]  WITH CHECK ADD FOREIGN KEY([country_id])
REFERENCES [dbo].[country] ([country_id])
GO
/****** Object:  StoredProcedure [dbo].[getMissions]    Script Date: 21-09-2024 23:08:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[getMissions] 
AS
SELECT * FROM mission INNER JOIN country ON mission.country_id = country.country_id
GO
USE [master]
GO
ALTER DATABASE [CommunityInvestment] SET  READ_WRITE 
GO
