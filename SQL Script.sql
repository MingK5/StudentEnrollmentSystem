USE [master]
GO
/****** Object:  Database [Group3]    Script Date: 2/5/2025 4:22:12 PM ******/
CREATE DATABASE [Group3]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Group3', FILENAME = N'C:\Users\PC\Group3.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Group3_log', FILENAME = N'C:\Users\PC\Group3_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [Group3] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Group3].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Group3] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Group3] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Group3] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Group3] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Group3] SET ARITHABORT OFF 
GO
ALTER DATABASE [Group3] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Group3] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Group3] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Group3] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Group3] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Group3] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Group3] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Group3] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Group3] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Group3] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Group3] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Group3] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Group3] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Group3] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Group3] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Group3] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Group3] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Group3] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Group3] SET  MULTI_USER 
GO
ALTER DATABASE [Group3] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Group3] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Group3] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Group3] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Group3] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Group3] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [Group3] SET QUERY_STORE = OFF
GO
USE [Group3]
GO
/****** Object:  Table [dbo].[Courses]    Script Date: 2/5/2025 4:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Courses](
	[courseId] [varchar](100) NOT NULL,
	[courseName] [varchar](255) NULL,
	[credit] [int] NULL,
	[lecturer] [varchar](255) NULL,
	[courseFee] [decimal](10, 2) NULL,
	[startTime] [time](7) NULL,
	[endTime] [time](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[courseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Enrol]    Script Date: 2/5/2025 4:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Enrol](
	[enrolId] [int] NOT NULL,
	[session] [varchar](100) NULL,
	[studentId] [varchar](100) NULL,
	[courseId] [varchar](100) NULL,
	[reason] [varchar](255) NULL,
	[action] [varchar](50) NULL,
	[status] [varchar](50) NULL,
	[datePerformed] [datetime] NULL,
	[evaluationId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[enrolId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Evaluation]    Script Date: 2/5/2025 4:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Evaluation](
	[evaluationId] [int] NOT NULL,
	[organizationRate] [int] NULL,
	[clarityRate] [int] NULL,
	[materialRate] [int] NULL,
	[comment] [text] NULL,
PRIMARY KEY CLUSTERED 
(
	[evaluationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feedback]    Script Date: 2/5/2025 4:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedback](
	[enquiryId] [int] NOT NULL,
	[studentId] [varchar](100) NULL,
	[category] [varchar](50) NULL,
	[subject] [varchar](255) NULL,
	[message] [text] NULL,
PRIMARY KEY CLUSTERED 
(
	[enquiryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Student]    Script Date: 2/5/2025 4:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Student](
	[studentId] [varchar](100) NOT NULL,
	[email] [varchar](255) NULL,
	[identificationNo] [varchar](100) NULL,
	[password] [varchar](255) NULL,
	[address] [varchar](255) NULL,
	[emergencyContactPerson] [varchar](255) NULL,
	[emergencyContactRelationship] [varchar](100) NULL,
	[emergencyContactHp] [varchar](50) NULL,
	[bankName] [varchar](255) NULL,
	[bankAccount] [varchar](100) NULL,
	[bankHolderName] [varchar](255) NULL,
	[studyMode] [varchar](50) NULL,
	[school] [varchar](255) NULL,
	[level] [varchar](50) NULL,
	[program] [varchar](100) NULL,
	[studentName] [varchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[studentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StudentAccount]    Script Date: 2/5/2025 4:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudentAccount](
	[transactionId] [int] NOT NULL,
	[transactionDate] [datetime] NULL,
	[studentId] [varchar](100) NULL,
	[process] [varchar](100) NULL,
	[particulars] [varchar](255) NULL,
	[documentNo] [varchar](100) NULL,
	[session] [varchar](50) NULL,
	[status] [varchar](50) NULL,
	[message] [text] NULL,
PRIMARY KEY CLUSTERED 
(
	[transactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StudentUnavailability]    Script Date: 2/5/2025 4:22:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudentUnavailability](
	[studentUnavailabilityId] [int] NOT NULL,
	[studentId] [varchar](100) NULL,
	[day] [varchar](50) NULL,
	[startTime] [time](7) NULL,
	[endTime] [time](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[studentUnavailabilityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Enrol]  WITH CHECK ADD FOREIGN KEY([courseId])
REFERENCES [dbo].[Courses] ([courseId])
GO
ALTER TABLE [dbo].[Enrol]  WITH CHECK ADD FOREIGN KEY([evaluationId])
REFERENCES [dbo].[Evaluation] ([evaluationId])
GO
ALTER TABLE [dbo].[Enrol]  WITH CHECK ADD FOREIGN KEY([studentId])
REFERENCES [dbo].[Student] ([studentId])
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD FOREIGN KEY([studentId])
REFERENCES [dbo].[Student] ([studentId])
GO
ALTER TABLE [dbo].[StudentAccount]  WITH CHECK ADD FOREIGN KEY([studentId])
REFERENCES [dbo].[Student] ([studentId])
GO
ALTER TABLE [dbo].[StudentUnavailability]  WITH CHECK ADD FOREIGN KEY([studentId])
REFERENCES [dbo].[Student] ([studentId])
GO
USE [master]
GO
ALTER DATABASE [Group3] SET  READ_WRITE 
GO
