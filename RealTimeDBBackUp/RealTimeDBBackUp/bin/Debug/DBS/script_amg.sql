USE [master]
GO
/****** Object:  Database [gitslotpark_amg]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE DATABASE [gitslotpark_amg]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'gitslotpark_amg', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\gitslotpark_amg.mdf' , SIZE = 429056KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ), 
 FILEGROUP [GAME_LOGS] 
( NAME = N'gitslotpark_amg_gamelog', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\gitslotpark_amg_gamelog' , SIZE = 850944KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'gitslotpark_amg_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\gitslotpark_amg_log.ldf' , SIZE = 20096KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [gitslotpark_amg] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [gitslotpark_amg].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [gitslotpark_amg] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [gitslotpark_amg] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [gitslotpark_amg] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [gitslotpark_amg] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [gitslotpark_amg] SET ARITHABORT OFF 
GO
ALTER DATABASE [gitslotpark_amg] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [gitslotpark_amg] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [gitslotpark_amg] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [gitslotpark_amg] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [gitslotpark_amg] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [gitslotpark_amg] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [gitslotpark_amg] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [gitslotpark_amg] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [gitslotpark_amg] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [gitslotpark_amg] SET  DISABLE_BROKER 
GO
ALTER DATABASE [gitslotpark_amg] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [gitslotpark_amg] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [gitslotpark_amg] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [gitslotpark_amg] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [gitslotpark_amg] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [gitslotpark_amg] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [gitslotpark_amg] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [gitslotpark_amg] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [gitslotpark_amg] SET  MULTI_USER 
GO
ALTER DATABASE [gitslotpark_amg] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [gitslotpark_amg] SET DB_CHAINING OFF 
GO
ALTER DATABASE [gitslotpark_amg] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [gitslotpark_amg] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [gitslotpark_amg] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [gitslotpark_amg] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [gitslotpark_amg] SET QUERY_STORE = OFF
GO
USE [gitslotpark_amg]
GO
/****** Object:  UserDefinedTableType [dbo].[AgentIDs]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE TYPE [dbo].[AgentIDs] AS TABLE(
	[id] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[AgentRollingTableType]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE TYPE [dbo].[AgentRollingTableType] AS TABLE(
	[rollpoint] [decimal](18, 2) NULL,
	[id] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[BalanceTableType]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE TYPE [dbo].[BalanceTableType] AS TABLE(
	[balance] [decimal](18, 2) NULL,
	[id] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[GameReportType]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE TYPE [dbo].[GameReportType] AS TABLE(
	[gameid] [int] NULL,
	[agentid] [int] NULL,
	[bet] [decimal](18, 2) NULL,
	[win] [decimal](18, 2) NULL,
	[reportdate] [datetime] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[PayoutPoolType]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE TYPE [dbo].[PayoutPoolType] AS TABLE(
	[agentid] [int] NULL,
	[bet] [decimal](18, 2) NULL,
	[win] [decimal](18, 2) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[ReportType]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE TYPE [dbo].[ReportType] AS TABLE(
	[username] [nvarchar](50) NULL,
	[bet] [decimal](18, 2) NULL,
	[win] [decimal](18, 2) NULL,
	[reporttime] [datetime] NULL,
	[agentid] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[UserBetMoneyTableType]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE TYPE [dbo].[UserBetMoneyTableType] AS TABLE(
	[id] [bigint] NULL,
	[betmoney] [decimal](18, 2) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[UserGameUpdateTable]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE TYPE [dbo].[UserGameUpdateTable] AS TABLE(
	[id] [int] NULL,
	[isonline] [int] NULL,
	[lastgame] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[UserLoginUpdateTable]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE TYPE [dbo].[UserLoginUpdateTable] AS TABLE(
	[id] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[UserNames]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE TYPE [dbo].[UserNames] AS TABLE(
	[username] [nvarchar](50) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[UserOfflineUpdateTable]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE TYPE [dbo].[UserOfflineUpdateTable] AS TABLE(
	[id] [int] NULL,
	[isonline] [int] NULL,
	[lastgame] [int] NULL,
	[balance] [decimal](18, 2) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[UserRollingTableType]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE TYPE [dbo].[UserRollingTableType] AS TABLE(
	[rollpoint] [decimal](18, 2) NULL,
	[id] [bigint] NULL
)
GO
/****** Object:  Table [dbo].[adminlogins]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[adminlogins](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[userid] [nvarchar](50) NOT NULL,
	[userpwd] [nvarchar](50) NOT NULL,
	[lastloginip] [nvarchar](150) NULL,
	[lastlogindate] [datetime] NULL,
 CONSTRAINT [PK_adminlogin] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[agentgameconfigs]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[agentgameconfigs](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[agentid] [int] NOT NULL,
	[gameid] [int] NOT NULL,
	[payoutrate] [decimal](18, 2) NOT NULL,
	[updatetime] [datetime] NOT NULL,
 CONSTRAINT [PK_agentgameconfigs] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[agentreports]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[agentreports](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[agentid] [int] NOT NULL,
	[bet] [decimal](18, 2) NOT NULL,
	[win] [decimal](18, 2) NOT NULL,
	[reporttime] [datetime] NOT NULL,
 CONSTRAINT [PK_agentreports] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[agents]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[agents](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](50) NOT NULL,
	[password] [nvarchar](50) NOT NULL,
	[secretkey] [varchar](128) NOT NULL,
	[callbackurl] [varchar](256) NOT NULL,
	[apitoken] [varchar](128) NULL,
	[nickname] [nvarchar](50) NOT NULL,
	[description] [nvarchar](2048) NULL,
	[state] [int] NOT NULL,
	[currency] [int] NOT NULL,
	[language] [int] NOT NULL,
	[updatetime] [datetime] NULL,
 CONSTRAINT [PK_agents] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[faildtransactions]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[faildtransactions](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[agentid] [nvarchar](50) NOT NULL,
	[userid] [varchar](50) NOT NULL,
	[transactiontype] [int] NOT NULL,
	[transactionid] [varchar](128) NOT NULL,
	[reftransactionid] [varchar](128) NOT NULL,
	[betamount] [decimal](18, 2) NOT NULL,
	[winamount] [decimal](18, 2) NOT NULL,
	[gameid] [int] NOT NULL,
	[timestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_faildtransactions] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[gameconfigs]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[gameconfigs](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[gameid] [int] NOT NULL,
	[gametype] [int] NOT NULL,
	[gamename] [nvarchar](150) NOT NULL,
	[gamesymbol] [nvarchar](50) NOT NULL,
	[payoutrate] [decimal](18, 2) NOT NULL,
	[openclose] [bit] NOT NULL,
	[updatetime] [datetime] NOT NULL,
	[releasedate] [datetime] NOT NULL,
 CONSTRAINT [PK_gameconfigs] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[gamereports](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[gameid] [int] NOT NULL,
	[agentid] [int] NOT NULL,
	[bet] [decimal](18, 2) NOT NULL,
	[win] [decimal](18, 2) NOT NULL,
	[reportdate] [datetime] NOT NULL,
 CONSTRAINT [PK_gamereports] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[payoutpoolconfig]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[payoutpoolconfig](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[agentid] [int] NOT NULL,
	[poolredundency] [decimal](18, 2) NOT NULL,
	[updatetime] [datetime] NOT NULL,
 CONSTRAINT [PK_payoutpoolconfig] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[payoutpoolresets]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[payoutpoolresets](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[agentid] [int] NOT NULL,
	[updatetime] [datetime] NOT NULL,
 CONSTRAINT [PK_payoutpoolresets] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[payoutpoolstatus]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[payoutpoolstatus](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[agentid] [int] NOT NULL,
	[totalbet] [decimal](18, 2) NOT NULL,
	[totalwin] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_payoutpoolstatus] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[quitusers]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[quitusers](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](50) NOT NULL,
	[updatetime] [datetime] NOT NULL,
 CONSTRAINT [PK_quitusers] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[transactions]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[transactions](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[agentid] [nvarchar](50) NOT NULL,
	[userid] [varchar](50) NOT NULL,
	[transactionid] [varchar](128) NOT NULL,
	[reltransactionid] [varchar](128) NULL,
	[platformtransactionid] [varchar](128) NULL,
	[transactiontype] [int] NOT NULL,
	[endtransaction] [bit] NOT NULL,
	[gameid] [int] NOT NULL,
	[betamount] [decimal](18, 2) NOT NULL,
	[winamount] [decimal](18, 2) NOT NULL,
	[timestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_transactions] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[userrangeevents]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[userrangeevents](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[agentid] [int] NOT NULL,
	[username] [nvarchar](50) NOT NULL,
	[agentname] [nvarchar](50) NOT NULL,
	[minodd] [decimal](18, 2) NOT NULL,
	[maxodd] [decimal](18, 2) NOT NULL,
	[maxbet] [decimal](18, 2) NOT NULL,
	[amount] [decimal](18, 2) NULL,
	[gamename] [nvarchar](50) NULL,
	[processed] [int] NOT NULL,
	[regdate] [datetime] NOT NULL,
	[processeddate] [datetime] NULL,
	[updatetime] [datetime] NULL,
 CONSTRAINT [PK_userrangeevents] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[userreports]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[userreports](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[agentid] [int] NOT NULL,
	[username] [nvarchar](50) NOT NULL,
	[bet] [decimal](18, 2) NOT NULL,
	[win] [decimal](18, 2) NOT NULL,
	[reporttime] [datetime] NOT NULL,
 CONSTRAINT [PK_userreports] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[agentid] [int] NOT NULL,
	[username] [nvarchar](50) NOT NULL,
	[password] [nvarchar](50) NOT NULL,
	[agentname] [nvarchar](50) NOT NULL,
	[balance] [decimal](18, 2) NOT NULL,
	[currency] [int] NOT NULL,
	[state] [int] NOT NULL,
	[isonline] [int] NOT NULL,
	[lastscorecounter] [bigint] NOT NULL,
	[lastgameid] [int] NOT NULL,
	[lastplatform] [int] NOT NULL,
	[lastbetmoney] [decimal](18, 2) NOT NULL,
	[lastonlinetime] [datetime] NULL,
	[updatetime] [datetime] NOT NULL,
	[registertime] [datetime] NOT NULL,
 CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [NonClusteredIndex-20230927-001019]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20230927-001019] ON [dbo].[agentreports]
(
	[agentid] ASC,
	[reporttime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [NonClusteredIndex-20230927-001040]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20230927-001040] ON [dbo].[agents]
(
	[username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [NonClusteredIndex-20231024-064153]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20231024-064153] ON [dbo].[gamereports]
(
	[gameid] ASC,
	[reportdate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [NonClusteredIndex-20231024-064207]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20231024-064207] ON [dbo].[gamereports]
(
	[agentid] ASC,
	[reportdate] ASC,
	[gameid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [NonClusteredIndex-20230712-163147]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20230712-163147] ON [dbo].[transactions]
(
	[agentid] ASC,
	[userid] ASC,
	[timestamp] ASC,
	[transactiontype] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [NonClusteredIndex-20230712-163226]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20230712-163226] ON [dbo].[transactions]
(
	[transactionid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [NonClusteredIndex-20240120-064409]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20240120-064409] ON [dbo].[userreports]
(
	[agentid] ASC,
	[username] ASC,
	[reporttime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [NonClusteredIndex-20230228-054051]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [NonClusteredIndex-20230228-054051] ON [dbo].[users]
(
	[agentid] ASC,
	[username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [NonClusteredIndex-20231030-083715]    Script Date: 4/21/2024 3:21:18 AM ******/
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20231030-083715] ON [dbo].[users]
(
	[isonline] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[agents] ADD  CONSTRAINT [DF_agents_state]  DEFAULT ((1)) FOR [state]
GO
ALTER TABLE [dbo].[agents] ADD  CONSTRAINT [DF_agents_updatetime]  DEFAULT (getdate()) FOR [updatetime]
GO
ALTER TABLE [dbo].[transactions] ADD  CONSTRAINT [DF_transactions_endtransaction]  DEFAULT ((0)) FOR [endtransaction]
GO
ALTER TABLE [dbo].[userrangeevents] ADD  CONSTRAINT [DF_userrangeevents_maxbet]  DEFAULT ((0.0)) FOR [maxbet]
GO
ALTER TABLE [dbo].[userrangeevents] ADD  CONSTRAINT [DF_userrangeevents_processed]  DEFAULT ((0)) FOR [processed]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_users_balance]  DEFAULT ((0.0)) FOR [balance]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_users_state]  DEFAULT ((1)) FOR [state]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_users_isonline]  DEFAULT ((0)) FOR [isonline]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_users_lastscoreid]  DEFAULT ((0)) FOR [lastscorecounter]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_users_lastgameid]  DEFAULT ((0)) FOR [lastgameid]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_users_lastplatform]  DEFAULT ((0)) FOR [lastplatform]
GO
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_users_lastbetmoney]  DEFAULT ((0.0)) FOR [lastbetmoney]
GO
/****** Object:  StoredProcedure [dbo].[UpdateBalance]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[UpdateBalance]
	@tblBalances BalanceTableType READONLY
AS
BEGIN
	SET NOCOUNT, XACT_ABORT ON;
	--UPDATE EXISTING RECORDS
	UPDATE users
	SET balance = p2.balance
	FROM users p1
	INNER JOIN @tblBalances p2 
	ON p1.id = p2.id
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateGameReports]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[UpdateGameReports]
	@tblGameReports GameReportType READONLY
AS
BEGIN
	SET NOCOUNT, XACT_ABORT ON;
	BEGIN TRY
	BEGIN TRANSACTION;
	MERGE gamereports WITH (HOLDLOCK) AS r1
	USING @tblGameReports AS r2
    ON r1.agentid = r2.agentid and r1.gameid = r2.gameid and r1.reportdate = r2.reportdate
	WHEN MATCHED THEN
		UPDATE SET r1.bet = r1.bet + r2.bet, r1.win=r1.win + r2.win
	WHEN NOT MATCHED THEN
		INSERT (gameid, agentid, bet, win,reportdate) VALUES (r2.gameid, r2.agentid, r2.bet, r2.win, r2.reportdate);
		
	COMMIT
	END TRY
	BEGIN CATCH
	if @@trancount > 0
	BEGIN
      ROLLBACK TRANSACTION
	END;
	THROW
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[UpdatePayoutPool]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[UpdatePayoutPool]
	@tblReports PayoutPoolType READONLY
AS
BEGIN
	SET NOCOUNT, XACT_ABORT ON;
	BEGIN TRY
	BEGIN TRANSACTION;
	MERGE payoutpoolstatus WITH (HOLDLOCK) AS r1
	USING @tblReports AS r2
    ON r1.agentid = r2.agentid
	WHEN MATCHED THEN
		UPDATE SET r1.totalbet = r2.bet, r1.totalwin = r2.win
	WHEN NOT MATCHED THEN
		INSERT (agentid, totalbet, totalwin) VALUES ( r2.agentid,r2.bet, r2.win);
		COMMIT
	END TRY
	BEGIN CATCH
	if @@trancount > 0
	BEGIN
      ROLLBACK TRANSACTION
	END;
	THROW
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateReports]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[UpdateReports]
	@tblReports ReportType READONLY
AS
BEGIN
	SET NOCOUNT, XACT_ABORT ON;
	BEGIN TRY
	BEGIN TRANSACTION;
	MERGE userreports WITH (HOLDLOCK) AS r1
	USING @tblReports AS r2
    ON r1.agentid= r2.agentid and r1.username = r2.username and r1.reporttime = r2.reporttime
	WHEN MATCHED THEN
		UPDATE SET r1.bet = r1.bet + r2.bet, r1.win = r1.win + r2.win
	WHEN NOT MATCHED THEN
		INSERT (agentid, username, bet, win, reporttime) VALUES ( r2.agentid, r2.username, r2.bet, r2.win, r2.reporttime);
	
	MERGE agentreports WITH (HOLDLOCK) AS r1
	USING (SELECT SUM(bet) as bet,SUM(win) as win, reporttime,agentid FROM @tblReports GROUP BY agentid,reporttime) AS r2
    ON r1.agentid = r2.agentid and r1.reporttime = r2.reporttime
	WHEN MATCHED THEN
		UPDATE SET r1.bet = r1.bet + r2.bet, r1.win = r1.win + r2.win
	WHEN NOT MATCHED THEN
		INSERT (agentid, bet, win, reporttime) VALUES (r2.agentid, r2.bet, r2.win, r2.reporttime);

	COMMIT
	END TRY
	BEGIN CATCH
	if @@trancount > 0
	BEGIN
      ROLLBACK TRANSACTION
	END;
	THROW
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateUserBetMoney]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[UpdateUserBetMoney]
	@tblBetMoney UserBetMoneyTableType READONLY
AS
BEGIN
	SET NOCOUNT, XACT_ABORT ON;
	--UPDATE EXISTING RECORDS
	UPDATE users
	SET lastbetmoney = p2.betmoney
	FROM users p1
	INNER JOIN @tblBetMoney p2 
	ON p1.id = p2.id
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateUserGame]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[UpdateUserGame]
	@tblUpdates UserGameUpdateTable READONLY
AS
BEGIN
	SET NOCOUNT ON;
	--UPDATE EXISTING RECORDS
	UPDATE users
	SET isonline=p2.isonline, lastgameid=p2.lastgame
	FROM users p1
	INNER JOIN @tblUpdates p2 
	ON p1.id = p2.id
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateUserOffline]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[UpdateUserOffline]
	@tblUpdates UserOfflineUpdateTable READONLY
AS
BEGIN
	SET NOCOUNT, XACT_ABORT ON;
	--UPDATE EXISTING RECORDS
	UPDATE users
	SET isonline=p2.isonline, lastgameid=p2.lastgame, balance=p1.balance + p2.balance
	FROM users p1
	INNER JOIN @tblUpdates p2 
	ON p1.id = p2.id
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateUserOnline]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[UpdateUserOnline]
	@tblUpdates UserLoginUpdateTable READONLY
AS
BEGIN
	SET NOCOUNT ON;
	--UPDATE EXISTING RECORDS
	UPDATE users
	SET isonline=1, lastonlinetime=GETUTCDATE()
	FROM users p1
	INNER JOIN @tblUpdates p2 
	ON p1.id = p2.id
END
GO
/****** Object:  StoredProcedure [dbo].[usp_CreateAgentLogTables]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE PROC [dbo].[usp_CreateAgentLogTables] (@agentid int) AS
-- =============================================
-- Create date: 18-10-2018
-- Description: dynamically create Ezugi or game logs
-- =============================================
BEGIN 

DECLARE @tablename VARCHAR(200)
,@SQLcmd VARCHAR(MAX)

SELECT @tablename = 'gamelog_'+CONVERT(VARCHAR,@agentid)

SELECT @SQLcmd= 
	'IF NOT EXISTS(SELECT name FROM sys.tables WHERE name = '''+@tablename+''') BEGIN 
	CREATE TABLE '+@tablename+'(
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](50) NOT NULL,
	[gameid] [int] NOT NULL,
	[gamename] [nvarchar](50) NOT NULL,
	[tableid] [nvarchar](50) NOT NULL,
	[bet] [decimal](18, 2) NOT NULL,
	[win] [decimal](18, 2) NOT NULL,
	[beginmoney] [decimal](18, 2) NOT NULL,
	[endmoney] [decimal](18, 2) NOT NULL,
	[gamelog] [nvarchar](max) NOT NULL,
	[logtime] [datetime] NOT NULL,
	CONSTRAINT [pk_'+@tablename+'_id] PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [GAME_LOGS]
	) ON [GAME_LOGS] TEXTIMAGE_ON [GAME_LOGS]; 	
	CREATE NONCLUSTERED INDEX [ix_'+@tablename+'_username_logtime] ON ' +QUOTENAME(@tablename)+'
    (
	    [username] ASC,
	    [logtime] ASC
    );
	CREATE INDEX [ix_'+@tablename+'_gamename_beginmoney_logtime_Includes] ON '+QUOTENAME(@tablename)+' 
	(
	[gamename], [beginmoney], [logtime]
	)  INCLUDE ([username], [endmoney]) WITH (FILLFACTOR=100, ONLINE=OFF
	);
	CREATE NONCLUSTERED INDEX [ix_'+@tablename+'_gameID] ON [dbo].'+QUOTENAME(@tablename)+'
(
	[gameid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [GAME_LOGS]
	END'


BEGIN TRY  
    --Run dynamic query to create table 
    EXECUTE (@SQLcmd)
	SELECT 1  
END TRY  
BEGIN CATCH  
    -- Execute error retrieval routine.  
	EXECUTE usp_GetErrorInfo; 

END CATCH; 
END





GO

/****** Object:  UserDefinedTableType [dbo].[BackupAgentGameConfigType]    Script Date: 4/21/2024 12:57:06 PM ******/
CREATE TYPE [dbo].[BackupAgentGameConfigType] AS TABLE(
	[agentid] [int] NULL,
	[gameid] [int] NULL,
	[payoutrate] [decimal](18, 2) NULL,
	[updatetime] [datetime] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[BackupAgentReportType]    Script Date: 4/21/2024 12:57:06 PM ******/
CREATE TYPE [dbo].[BackupAgentReportType] AS TABLE(
	[agentid] [int] NULL,
	[bet] [decimal](18, 2) NULL,
	[win] [decimal](18, 2) NULL,
	[reporttime] [datetime] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[BackupAgentType]    Script Date: 4/21/2024 12:57:06 PM ******/
CREATE TYPE [dbo].[BackupAgentType] AS TABLE(
	[id] [int] NULL,
	[username] [nvarchar](50) NULL,
	[password] [nvarchar](50) NULL,
	[secretkey] [nvarchar](128) NULL,
	[callbackurl] [nvarchar](512) NULL,
	[apitoken] [nvarchar](128) NULL,
	[nickname] [nvarchar](50) NULL,
	[state] [int] NULL,
	[currency] [int] NULL,
	[language] [int] NULL,
	[updatetime] [datetime] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[BackupGameConfigType]    Script Date: 4/21/2024 12:57:06 PM ******/
CREATE TYPE [dbo].[BackupGameConfigType] AS TABLE(
	[gameid] [int] NULL,
	[gametype] [int] NULL,
	[gamename] [nvarchar](255) NULL,
	[gamesymbol] [nvarchar](255) NULL,
	[payoutrate] [decimal](18, 2) NULL,
	[openclose] [bit] NULL,
	[updatetime] [datetime] NULL,
	[releasedate] [datetime] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[BackupGameReportType]    Script Date: 4/21/2024 12:57:06 PM ******/
CREATE TYPE [dbo].[BackupGameReportType] AS TABLE(
	[gameid] [int] NULL,
	[agentid] [int] NULL,
	[bet] [decimal](18, 2) NULL,
	[win] [decimal](18, 2) NULL,
	[reportdate] [datetime] NULL
)
GO
/****** Object:  StoredProcedure [dbo].[BackupAgentGameConfigs]    Script Date: 4/21/2024 12:57:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[BackupAgentGameConfigs]
	@tblGameConfigs BackupAgentGameConfigType READONLY
AS
BEGIN
	SET NOCOUNT, XACT_ABORT ON;
	BEGIN TRY
	BEGIN TRANSACTION;
	MERGE agentgameconfigs WITH (HOLDLOCK) AS g1
	USING @tblGameConfigs AS g2
    ON g1.gameid = g2.gameid AND g1.agentid = g2.agentid
	WHEN MATCHED THEN
		UPDATE SET g1.payoutrate = g2.payoutrate,g1.updatetime = g2.updatetime
	WHEN NOT MATCHED THEN
		INSERT (agentid,gameid,payoutrate,updatetime) 
		VALUES (g2.agentid,g2.gameid,g2.payoutrate,g2.updatetime);
	COMMIT
	END TRY
	BEGIN CATCH
	if @@trancount > 0
	BEGIN
      ROLLBACK TRANSACTION
	END;
	THROW
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[BackupAgentReports]    Script Date: 4/21/2024 12:57:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[BackupAgentReports]
	@tblReports BackupAgentReportType READONLY
AS
BEGIN
	SET NOCOUNT, XACT_ABORT ON;
	BEGIN TRY
	BEGIN TRANSACTION;
	MERGE agentreports WITH (HOLDLOCK) AS g1
	USING @tblReports AS g2
    ON g1.agentid = g2.agentid AND g1.reporttime = g2.reporttime
	WHEN MATCHED THEN
		UPDATE SET g1.bet = g2.bet,g1.win=g2.win
	WHEN NOT MATCHED THEN
		INSERT (agentid,bet,win,reporttime) 
		VALUES (g2.agentid,g2.bet,g2.win,g2.reporttime);
	COMMIT
	END TRY
	BEGIN CATCH
	if @@trancount > 0
	BEGIN
      ROLLBACK TRANSACTION
	END;
	THROW
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[BackupAgents]    Script Date: 4/21/2024 12:57:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[BackupAgents]
	@tblAgents BackupAgentType READONLY
AS
BEGIN
	SET NOCOUNT, XACT_ABORT ON;
	BEGIN TRY
	BEGIN TRANSACTION;
	SET IDENTITY_INSERT agents ON;
	MERGE agents WITH (HOLDLOCK) AS g1
	USING @tblAgents AS g2
    ON g1.username = g2.username
	WHEN MATCHED THEN
		UPDATE SET g1.password = g2.password,g1.secretkey=g2.secretkey,g1.callbackurl=g2.callbackurl,g1.apitoken=g2.apitoken,g1.state=g2.state, g1.updatetime = g2.updatetime
	WHEN NOT MATCHED THEN
		INSERT (id,username,password,secretkey,callbackurl,apitoken,nickname,state,currency,language,updatetime) 
		VALUES (g2.id,g2.username,g2.password,g2.secretkey,g2.callbackurl,g2.apitoken,g2.nickname,g2.state,g2.currency,g2.language,g2.updatetime);
	SET IDENTITY_INSERT agents OFF;
	COMMIT
	END TRY
	BEGIN CATCH
	if @@trancount > 0
	BEGIN
      ROLLBACK TRANSACTION
	END;
	THROW
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[BackupGameConfigs]    Script Date: 4/21/2024 12:57:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[BackupGameConfigs]
	@tblGameConfigs BackupGameConfigType READONLY
AS
BEGIN
	SET NOCOUNT, XACT_ABORT ON;
	BEGIN TRY
	BEGIN TRANSACTION;
	MERGE gameconfigs WITH (HOLDLOCK) AS g1
	USING @tblGameConfigs AS g2
    ON g1.gameid = g2.gameid
	WHEN MATCHED THEN
		UPDATE SET g1.payoutrate = g2.payoutrate, g1.updatetime = g2.updatetime
	WHEN NOT MATCHED THEN
		INSERT (gameid,gametype,gamename,gamesymbol,payoutrate,openclose,updatetime,releasedate) 
		VALUES (g2.gameid,g2.gametype,g2.gamename,g2.gamesymbol,g2.payoutrate,g2.openclose,g2.updatetime,g2.releasedate);
	COMMIT
	END TRY
	BEGIN CATCH
	if @@trancount > 0
	BEGIN
      ROLLBACK TRANSACTION
	END;
	THROW
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[BackupGameReports]    Script Date: 4/21/2024 12:57:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[BackupGameReports]
	@tblReports BackupGameReportType READONLY
AS
BEGIN
	SET NOCOUNT, XACT_ABORT ON;
	BEGIN TRY
	BEGIN TRANSACTION;
	MERGE gamereports WITH (HOLDLOCK) AS g1
	USING @tblReports AS g2
    ON g1.gameid = g2.gameid AND g1.agentid = g2.agentid AND g1.reportdate = g2.reportdate
	WHEN MATCHED THEN
		UPDATE SET g1.bet = g2.bet,g1.win=g2.win
	WHEN NOT MATCHED THEN
		INSERT (gameid,agentid,bet,win,reportdate) 
		VALUES (gameid,g2.agentid,g2.bet,g2.win,g2.reportdate);
	COMMIT
	END TRY
	BEGIN CATCH
	if @@trancount > 0
	BEGIN
      ROLLBACK TRANSACTION
	END;
	THROW
	END CATCH
END
GO

/****** Object:  StoredProcedure [dbo].[usp_GetErrorInfo]    Script Date: 4/21/2024 3:21:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Create procedure to retrieve error information.  
CREATE PROCEDURE [dbo].[usp_GetErrorInfo]  
AS  
SELECT  
    ERROR_NUMBER() AS ErrorNumber  
    ,ERROR_SEVERITY() AS ErrorSeverity  
    ,ERROR_STATE() AS ErrorState  
    ,ERROR_PROCEDURE() AS ErrorProcedure  
    ,ERROR_LINE() AS ErrorLine  
    ,ERROR_MESSAGE() AS ErrorMessage;  
GO
USE [master]
GO
ALTER DATABASE [gitslotpark_amg] SET  READ_WRITE 
GO
