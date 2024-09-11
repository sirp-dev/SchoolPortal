namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class helpdata : DbMigration
    {
        public override void Up()
        {


            Sql("SET IDENTITY_INSERT [dbo].[Helps] ON");


            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1002, N'HOW TO CREATE A NEW SESSION', N'http://support.esp.exwhyzee.ng/Home/Details/1', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1003, N'HOW TO MOVE TO NEXT SESSION AND PREVIOUS SESSION', N'http://support.esp.exwhyzee.ng/Home/Details/2', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1004, N'HOW TO CREATE A NEW CLASS AND EDIT A CLASS', N'http://support.esp.exwhyzee.ng/Home/Details/3', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1005, N'HOW TO ADD A NEW STUDENT/PUPIL', N'http://support.esp.exwhyzee.ng/Home/Details/5', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1006, N'HOW TO ADD A NEW STAFF', N'http://support.esp.exwhyzee.ng/Home/Details/6', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1007, N'HOW TO ENROLL A STUDENT/PUPIL TO A CLASS', N'http://support.esp.exwhyzee.ng/Home/Details/8', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1008, N'HOW TO ADD A NEW STUDENT', N'https://www.youtube.com/watch?v=UzB81NpG74U', 2, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1009, N'HOW TO UN-ENROLL OR REMOVE A STUDENT/PUPIL FROM A CLASS', N'http://support.esp.exwhyzee.ng/Home/Details/9', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1010, N'HOW TO VIEW THE LIST OF ENROLLED STUDENTS/PUPILS', N'http://support.esp.exwhyzee.ng/Home/Details/10', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1011, N'HOW TO MANAGE RESULTS IN A CLASS', N'http://support.esp.exwhyzee.ng/Home/Details/11', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1012, N'HOW TO MANAGE STUDENT/PUPIL INFORMATION', N'http://support.esp.exwhyzee.ng/Home/Details/12', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1013, N'HOW TO VIEW STUDENT/PUPIL RESULT DETAILS', N'http://support.esp.exwhyzee.ng/Home/Details/13', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1014, N'HOW TO MANAGE A PARTICULAR SUBJECT SCORE IN A CLASS', N'http://support.esp.exwhyzee.ng/Home/Details/14', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1015, N'HOW TO ENTER STUDENT/PUPIL SUBJECT SCORE', N'http://support.esp.exwhyzee.ng/Home/Details/15', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1016, N'HOW TO PRINT ASSESSMENT AND EXAM SCORE SHEET', N'http://support.esp.exwhyzee.ng/Home/Details/16', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1017, N'HOW TO ENTER STUDENT/PUPIL SUBJECT SCORE OFFLINE', N'http://support.esp.exwhyzee.ng/Home/Details/17', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1018, N'HOW TO PREVIEW ALL STUDENT/PUPIL RESULT IN A CLASS', N'http://support.esp.exwhyzee.ng/Home/Details/18', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1019, N'HOW TO PRINT STUDENT/PUPIL RESULT', N'http://support.esp.exwhyzee.ng/Home/Details/19', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1020, N'HOW TO PRINT STUDENT/PUPIL RESULTS IN BATCH', N'http://support.esp.exwhyzee.ng/Home/Details/20', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1021, N'HOW TO VIEW AND PRINT THE MASTER LIST OF A CLASS', N'http://support.esp.exwhyzee.ng/Home/Details/21', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1022, N'HOW TO MANAGE ALL PIN CARDS', N'http://support.esp.exwhyzee.ng/Home/Details/22', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1023, N'HOW TO VIEW THE LIST OF ALL STUDENTS/PUPILS IN A CLASS AND ALSO EDIT AND REMOVE STUDENT/PUPIL IN A CLASS', N'http://support.esp.exwhyzee.ng/Home/Details/23', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1024, N'HOW TO CREATE AND MANAGE POST/CONTENT', N'http://support.esp.exwhyzee.ng/Home/Details/24', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1025, N'HOW TO CHANGE STUDENT/PUPIL STATUS', N'http://support.esp.exwhyzee.ng/Home/Details/25', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1026, N'HOW TO BLOCK A DEFAULTING STUDENT/PUPIL', N'http://support.esp.exwhyzee.ng/Home/Details/26', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1027, N'HOW TO CHANGE STAFF STATUS', N'http://support.esp.exwhyzee.ng/Home/Details/27', 1, 4)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1028, N'HOW TO CREATE A NEW SESSION', N'https://www.youtube.com/watch?v=wAy3GZqvSKw', 2, 3)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1029, N'HOW TO MOVE TO PREVIOUS AND NEXT TERM SESSION', N'https://www.youtube.com/watch?v=dEEZdxZ_WcQ', 2, 2)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1030, N'HOW TO ADD A NEW CLASS', N'https://www.youtube.com/watch?v=xf2iF2nvj_8', 2, 1)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1031, N'HOW TO EDIT A CLASS', N'https://www.youtube.com/watch?v=XWPiy1UCnO4', 2, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1032, N'HOW TO ADD HALL OF FAME', N'http://support.esp.exwhyzee.ng/Home/Details/28', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1033, N'HOW TO DELETE A CLASS', N'https://www.youtube.com/watch?v=b0FwXnBu8wU', 2, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1034, N'OVERVIEW OF THE ADMIN DASHBOARD', N'http://support.esp.exwhyzee.ng/Home/Details/29', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1035, N'SCHOOL PORTAL SETTINGS', N'http://support.esp.exwhyzee.ng/Home/Details/30', 1, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1036, N'HOW TO ADD A NEW SUBJECT IN A CLASS', N'https://www.youtube.com/watch?v=Zx44S6gpS14', 2, 0)");
            Sql("INSERT [dbo].[Helps] ([Id], [Title], [HelpUrl], [Type], [SortOrder]) VALUES (1037, N'HOW TO ADD AND UPDATE GRADING SETTINGS', N'http://support.esp.exwhyzee.ng/Home/Details/31', 1, 0)");
            Sql("SET IDENTITY_INSERT [dbo].[Helps] OFF");




        }

        public override void Down()
        {
        }
    }
}


