namespace SchoolPortal.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Intialmigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssignmentAnswers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AssignmentId = c.Int(),
                        DateAnswered = c.DateTime(nullable: false),
                        DateModified = c.DateTime(),
                        UserId = c.String(maxLength: 128),
                        StudentId = c.Int(),
                        ClassId = c.Int(),
                        EnrollementId = c.Int(),
                        Answer = c.String(),
                        Assessed = c.Boolean(nullable: false),
                        ClassLevel_Id = c.Int(),
                        StudentProfile_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Assignments", t => t.AssignmentId)
                .ForeignKey("dbo.ClassLevels", t => t.ClassLevel_Id)
                .ForeignKey("dbo.Enrollments", t => t.EnrollementId)
                .ForeignKey("dbo.StudentProfiles", t => t.StudentProfile_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.AssignmentId)
                .Index(t => t.UserId)
                .Index(t => t.EnrollementId)
                .Index(t => t.ClassLevel_Id)
                .Index(t => t.StudentProfile_Id);
            
            CreateTable(
                "dbo.Assignments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClassLevelId = c.Int(),
                        SessionId = c.Int(),
                        SubjectId = c.Int(),
                        Title = c.String(),
                        Description = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateSubmitionEnds = c.DateTime(),
                        IsPublished = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClassLevels", t => t.ClassLevelId)
                .ForeignKey("dbo.Sessions", t => t.SessionId)
                .ForeignKey("dbo.Subjects", t => t.SubjectId)
                .Index(t => t.ClassLevelId)
                .Index(t => t.SessionId)
                .Index(t => t.SubjectId);
            
            CreateTable(
                "dbo.ClassLevels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClassName = c.String(nullable: false),
                        UserId = c.String(maxLength: 128),
                        SessionId = c.Int(),
                        StaffProfile_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sessions", t => t.SessionId)
                .ForeignKey("dbo.StaffProfiles", t => t.StaffProfile_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.SessionId)
                .Index(t => t.StaffProfile_Id);
            
            CreateTable(
                "dbo.Attendances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        UserId = c.String(maxLength: 128),
                        ClassLevelId = c.Int(),
                        EnrollmentId = c.Int(),
                        SessionId = c.Int(),
                        Updated = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClassLevels", t => t.ClassLevelId)
                .ForeignKey("dbo.Enrollments", t => t.EnrollmentId)
                .ForeignKey("dbo.Sessions", t => t.SessionId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ClassLevelId)
                .Index(t => t.EnrollmentId)
                .Index(t => t.SessionId);
            
            CreateTable(
                "dbo.AttendanceDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        StudentId = c.Int(),
                        SessionId = c.Int(),
                        IsPresent = c.Boolean(nullable: false),
                        EnrollmentId = c.Int(),
                        AttendanceId = c.Int(),
                        StudentProfile_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Attendances", t => t.AttendanceId)
                .ForeignKey("dbo.Enrollments", t => t.EnrollmentId)
                .ForeignKey("dbo.StudentProfiles", t => t.StudentProfile_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.EnrollmentId)
                .Index(t => t.AttendanceId)
                .Index(t => t.StudentProfile_Id);
            
            CreateTable(
                "dbo.Enrollments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClassLevelId = c.Int(),
                        StudentProfileId = c.Int(nullable: false),
                        SessionId = c.Int(nullable: false),
                        AverageScore = c.Decimal(precision: 18, scale: 2),
                        CummulativeAverageScore = c.Decimal(precision: 18, scale: 2),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClassLevels", t => t.ClassLevelId)
                .ForeignKey("dbo.Sessions", t => t.SessionId, cascadeDelete: true)
                .ForeignKey("dbo.StudentProfiles", t => t.StudentProfileId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.ClassLevelId)
                .Index(t => t.StudentProfileId)
                .Index(t => t.SessionId)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.EnrolledSubjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SubjectId = c.Int(),
                        TestScore = c.Decimal(precision: 18, scale: 2),
                        ExamScore = c.Decimal(precision: 18, scale: 2),
                        TotalScore = c.Decimal(precision: 18, scale: 2),
                        EnrollmentId = c.Int(nullable: false),
                        GradingOption = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Enrollments", t => t.EnrollmentId, cascadeDelete: true)
                .ForeignKey("dbo.Subjects", t => t.SubjectId)
                .Index(t => t.SubjectId)
                .Index(t => t.EnrollmentId);
            
            CreateTable(
                "dbo.Subjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SubjectName = c.String(nullable: false),
                        ClassLevelId = c.Int(nullable: false),
                        ExamScore = c.Decimal(precision: 18, scale: 2),
                        TestScore = c.Decimal(precision: 18, scale: 2),
                        PassMark = c.Decimal(precision: 18, scale: 2),
                        RequiresPass = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClassLevels", t => t.ClassLevelId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.ClassLevelId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Surname = c.String(),
                        FirstName = c.String(),
                        OtherName = c.String(),
                        DateOfBirth = c.DateTime(),
                        DateRegistered = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        DataStatusChanged = c.DateTime(),
                        Religion = c.String(),
                        Gender = c.String(),
                        ContactAddress = c.String(),
                        City = c.String(),
                        Phone = c.String(),
                        LocalGov = c.String(),
                        StateOfOrigin = c.String(),
                        Nationality = c.String(),
                        RegisteredBy = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Sessions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Term = c.String(),
                        SchoolPrincipal = c.String(nullable: false),
                        SessionYear = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StudentProfiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        Disability = c.String(),
                        EmergencyContact = c.String(),
                        StudentRegNumber = c.String(),
                        AboutMe = c.String(),
                        FavouriteFood = c.String(),
                        BooksYouLike = c.String(),
                        MoviesYouLike = c.String(),
                        TVShowsYouLike = c.String(),
                        YourLikes = c.String(),
                        YourDisLikes = c.String(),
                        FavouriteColour = c.String(),
                        LastPrimarySchoolAttended = c.String(),
                        ParentGuardianName = c.String(),
                        ParentGuardianAddress = c.String(),
                        ParentGuardianPhoneNumber = c.String(),
                        ParentGuardianOccupation = c.String(),
                        ImageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.StaffProfiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        Disability = c.String(),
                        EmergencyContact = c.String(),
                        MaritalStatus = c.String(),
                        AboutMe = c.String(),
                        FavouriteFood = c.String(),
                        BooksYouLike = c.String(),
                        MoviesYouLike = c.String(),
                        TVShowsYouLike = c.String(),
                        YourLikes = c.String(),
                        YourDisLikes = c.String(),
                        FavouriteColour = c.String(),
                        StaffRegistrationId = c.String(),
                        DateOfAppointment = c.DateTime(nullable: false),
                        PostHeld = c.String(),
                        ImageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Qualifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NameOfInstitution = c.String(nullable: false),
                        YearObtained = c.String(nullable: false),
                        CertificateObtained = c.String(nullable: false),
                        StaffId = c.Int(nullable: false),
                        UserId = c.String(),
                        StaffProfile_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StaffProfiles", t => t.StaffProfile_Id)
                .Index(t => t.StaffProfile_Id);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Content = c.String(),
                        DateCommented = c.DateTime(nullable: false),
                        PostId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: true)
                .Index(t => t.PostId);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 100),
                        Content = c.String(nullable: false),
                        DatePosted = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        WhoCanSeePost = c.Int(nullable: false),
                        PageType = c.Int(nullable: false),
                        PostedBy = c.String(),
                        SortOrder = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PostImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        ContentType = c.String(),
                        ImageContent = c.Binary(),
                        PostId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: true)
                .Index(t => t.PostId);
            
            CreateTable(
                "dbo.ContactUs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        PhoneNumber = c.Int(nullable: false),
                        Email = c.String(nullable: false),
                        Subject = c.String(nullable: false),
                        Message = c.String(nullable: false),
                        UserId = c.Int(nullable: false),
                        messageStatus = c.Int(nullable: false),
                        user_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.user_Id)
                .Index(t => t.user_Id);
            
            CreateTable(
                "dbo.MessageReplies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReplyMessage = c.String(),
                        MessageId = c.Int(nullable: false),
                        userId = c.String(maxLength: 128),
                        ReplyDate = c.DateTime(nullable: false),
                        ContactUs_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ContactUs", t => t.ContactUs_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.userId)
                .Index(t => t.userId)
                .Index(t => t.ContactUs_Id);
            
            CreateTable(
                "dbo.Defaulters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Reason = c.String(),
                        ProfileId = c.Int(nullable: false),
                        StudentProfile_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StudentProfiles", t => t.StudentProfile_Id)
                .Index(t => t.StudentProfile_Id);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Subject = c.String(),
                        DIscription = c.String(),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(),
                        Color = c.String(),
                        UserId = c.String(),
                        GeneralEvent = c.Boolean(),
                        IsFullDay = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GradingDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GradingId = c.Int(nullable: false),
                        UpperLimit = c.Int(nullable: false),
                        LowerLimit = c.Int(nullable: false),
                        Grade = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Gradings", t => t.GradingId, cascadeDelete: true)
                .Index(t => t.GradingId);
            
            CreateTable(
                "dbo.Gradings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HallOfFames",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        SortOrder = c.Int(nullable: false),
                        Image = c.Binary(),
                        Content = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ImageModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        ContentType = c.String(),
                        ImageContent = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ImageSliders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        ContentType = c.String(),
                        Content = c.Binary(),
                        SliderAlt = c.String(),
                        CurrentSlider = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LocalGovs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LGAName = c.String(),
                        StatesId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.States", t => t.StatesId)
                .Index(t => t.StatesId);
            
            CreateTable(
                "dbo.States",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StateName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PinCodeModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PinNumber = c.String(),
                        SerialNumber = c.String(),
                        Usage = c.Int(nullable: false),
                        EnrollmentId = c.Int(),
                        BatchNumber = c.String(),
                        StudentPin = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Properties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UnitsPerSms = c.Decimal(precision: 18, scale: 2),
                        UnitsInAccount = c.Decimal(precision: 18, scale: 2),
                        SmsUsername = c.String(),
                        SmsPassword = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PublishResults",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SessionId = c.Int(nullable: false),
                        ClassLevelId = c.Int(nullable: false),
                        PublishedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClassLevels", t => t.ClassLevelId, cascadeDelete: true)
                .ForeignKey("dbo.Sessions", t => t.SessionId, cascadeDelete: true)
                .Index(t => t.SessionId)
                .Index(t => t.ClassLevelId);
            
            CreateTable(
                "dbo.Quotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateDisplayed = c.DateTime(),
                        QuoteOfTheDay = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Riddles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        UserAnswer = c.String(),
                        CorrectAnswer = c.String(),
                        UserId = c.String(maxLength: 128),
                        DateAnswered = c.DateTime(),
                        TimeCountDown = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SchoolName = c.String(),
                        SchoolInitials = c.String(),
                        SchoolAddress = c.String(),
                        SchoolAddress2 = c.String(),
                        State = c.String(),
                        Country = c.String(),
                        ContactEmail = c.String(),
                        ContactPhoneNumber = c.String(),
                        ImageId = c.Int(nullable: false),
                        SmsUsername = c.String(),
                        SmsPassword = c.String(),
                        Passmark = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PromotionByTrial = c.Decimal(nullable: false, precision: 18, scale: 2),
                        JssGradingOption = c.Int(nullable: false),
                        SssGradingOption = c.Int(nullable: false),
                        ShowPositionOnResult = c.Boolean(nullable: false),
                        ShowCumulativeResultForThirdTerm = c.Boolean(nullable: false),
                        UseCumulativePositionForThirdTerm = c.Boolean(nullable: false),
                        EmailFrom = c.String(),
                        MailHost = c.String(),
                        Port = c.String(),
                        MailUsername = c.String(),
                        MailPassword = c.String(),
                        SslEnabled = c.Boolean(nullable: false),
                        AccessmentScore = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExamScore = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ScreeningVenue = c.String(),
                        ScreeningTime = c.String(),
                        ScreeningDate = c.String(),
                        WebsiteLink = c.String(),
                        PortalLink = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SmsGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupName = c.String(),
                        Numbers = c.String(),
                        NumbersCount = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SmsReports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SendTo = c.String(),
                        SenderId = c.String(maxLength: 11),
                        GroupName = c.String(),
                        Message = c.String(),
                        Comment = c.String(),
                        DateSent = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StudentDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RegistrationNumber = c.String(),
                        Surname = c.String(nullable: false),
                        FirstName = c.String(nullable: false),
                        OtherNames = c.String(),
                        DateOfBirth = c.DateTime(nullable: false),
                        LastPrimarySchoolAttended = c.String(nullable: false),
                        Religion = c.String(nullable: false),
                        NameOfParents = c.String(nullable: false),
                        ParentsAddress = c.String(nullable: false),
                        PhoneNumberOfParents = c.String(nullable: false),
                        OccupationOfParents = c.String(nullable: false),
                        PermanentHomeAddress = c.String(nullable: false),
                        LocalGov = c.String(),
                        StateOfOrigin = c.String(),
                        Nationality = c.String(nullable: false),
                        ExamCenter = c.String(),
                        Disability = c.String(),
                        EmergencyContact = c.String(nullable: false),
                        ExamScore = c.Decimal(precision: 18, scale: 2),
                        Remarks = c.String(),
                        AdmissionNumber = c.String(),
                        Status = c.Int(nullable: false),
                        DateOfAdmission = c.DateTime(),
                        AdmissionOfficer = c.String(),
                        ApplicationDate = c.DateTime(),
                        ImageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Syllables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Content = c.String(),
                        SessionId = c.Int(),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sessions", t => t.SessionId)
                .Index(t => t.SessionId);
            
            CreateTable(
                "dbo.TimeTables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ClassLevelId = c.Int(),
                        Monday = c.String(),
                        M6_7 = c.String(),
                        M7_8 = c.String(),
                        M8_9 = c.String(),
                        M9_10 = c.String(),
                        M10_11 = c.String(),
                        M11_12 = c.String(),
                        M12_13 = c.String(),
                        M13_14 = c.String(),
                        M14_15 = c.String(),
                        M15_16 = c.String(),
                        M16_17 = c.String(),
                        M17_18 = c.String(),
                        Tuesday = c.String(),
                        T6_7 = c.String(),
                        T7_8 = c.String(),
                        T8_9 = c.String(),
                        T9_10 = c.String(),
                        T10_11 = c.String(),
                        T11_12 = c.String(),
                        T12_13 = c.String(),
                        T13_14 = c.String(),
                        T14_15 = c.String(),
                        T15_16 = c.String(),
                        T16_17 = c.String(),
                        T17_18 = c.String(),
                        Wednessday = c.String(),
                        W6_7 = c.String(),
                        W7_8 = c.String(),
                        W8_9 = c.String(),
                        W9_10 = c.String(),
                        W10_11 = c.String(),
                        W11_12 = c.String(),
                        W12_13 = c.String(),
                        W13_14 = c.String(),
                        W14_15 = c.String(),
                        W15_16 = c.String(),
                        W16_17 = c.String(),
                        W17_18 = c.String(),
                        Thursday = c.String(),
                        Th6_7 = c.String(),
                        Th7_8 = c.String(),
                        Th8_9 = c.String(),
                        Th9_10 = c.String(),
                        Th10_11 = c.String(),
                        Th11_12 = c.String(),
                        Th12_13 = c.String(),
                        Th13_14 = c.String(),
                        Th14_15 = c.String(),
                        Th15_16 = c.String(),
                        Th16_17 = c.String(),
                        Th17_18 = c.String(),
                        Friday = c.String(),
                        F6_7 = c.String(),
                        F7_8 = c.String(),
                        F8_9 = c.String(),
                        F9_10 = c.String(),
                        F10_11 = c.String(),
                        F11_12 = c.String(),
                        F12_13 = c.String(),
                        F13_14 = c.String(),
                        F14_15 = c.String(),
                        F15_16 = c.String(),
                        F16_17 = c.String(),
                        F17_18 = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClassLevels", t => t.ClassLevelId)
                .Index(t => t.ClassLevelId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TimeTables", "ClassLevelId", "dbo.ClassLevels");
            DropForeignKey("dbo.Syllables", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Riddles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PublishResults", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.PublishResults", "ClassLevelId", "dbo.ClassLevels");
            DropForeignKey("dbo.LocalGovs", "StatesId", "dbo.States");
            DropForeignKey("dbo.GradingDetails", "GradingId", "dbo.Gradings");
            DropForeignKey("dbo.Defaulters", "StudentProfile_Id", "dbo.StudentProfiles");
            DropForeignKey("dbo.ContactUs", "user_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.MessageReplies", "userId", "dbo.AspNetUsers");
            DropForeignKey("dbo.MessageReplies", "ContactUs_Id", "dbo.ContactUs");
            DropForeignKey("dbo.PostImages", "PostId", "dbo.Posts");
            DropForeignKey("dbo.Comments", "PostId", "dbo.Posts");
            DropForeignKey("dbo.AssignmentAnswers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AssignmentAnswers", "StudentProfile_Id", "dbo.StudentProfiles");
            DropForeignKey("dbo.AssignmentAnswers", "EnrollementId", "dbo.Enrollments");
            DropForeignKey("dbo.AssignmentAnswers", "ClassLevel_Id", "dbo.ClassLevels");
            DropForeignKey("dbo.Assignments", "SubjectId", "dbo.Subjects");
            DropForeignKey("dbo.Assignments", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.Assignments", "ClassLevelId", "dbo.ClassLevels");
            DropForeignKey("dbo.ClassLevels", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ClassLevels", "StaffProfile_Id", "dbo.StaffProfiles");
            DropForeignKey("dbo.StaffProfiles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Qualifications", "StaffProfile_Id", "dbo.StaffProfiles");
            DropForeignKey("dbo.ClassLevels", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.Attendances", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Attendances", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.Attendances", "EnrollmentId", "dbo.Enrollments");
            DropForeignKey("dbo.Attendances", "ClassLevelId", "dbo.ClassLevels");
            DropForeignKey("dbo.AttendanceDetails", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AttendanceDetails", "StudentProfile_Id", "dbo.StudentProfiles");
            DropForeignKey("dbo.AttendanceDetails", "EnrollmentId", "dbo.Enrollments");
            DropForeignKey("dbo.Enrollments", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Enrollments", "StudentProfileId", "dbo.StudentProfiles");
            DropForeignKey("dbo.StudentProfiles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Enrollments", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.EnrolledSubjects", "SubjectId", "dbo.Subjects");
            DropForeignKey("dbo.Subjects", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Subjects", "ClassLevelId", "dbo.ClassLevels");
            DropForeignKey("dbo.EnrolledSubjects", "EnrollmentId", "dbo.Enrollments");
            DropForeignKey("dbo.Enrollments", "ClassLevelId", "dbo.ClassLevels");
            DropForeignKey("dbo.AttendanceDetails", "AttendanceId", "dbo.Attendances");
            DropForeignKey("dbo.AssignmentAnswers", "AssignmentId", "dbo.Assignments");
            DropIndex("dbo.TimeTables", new[] { "ClassLevelId" });
            DropIndex("dbo.Syllables", new[] { "SessionId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Riddles", new[] { "UserId" });
            DropIndex("dbo.PublishResults", new[] { "ClassLevelId" });
            DropIndex("dbo.PublishResults", new[] { "SessionId" });
            DropIndex("dbo.LocalGovs", new[] { "StatesId" });
            DropIndex("dbo.GradingDetails", new[] { "GradingId" });
            DropIndex("dbo.Defaulters", new[] { "StudentProfile_Id" });
            DropIndex("dbo.MessageReplies", new[] { "ContactUs_Id" });
            DropIndex("dbo.MessageReplies", new[] { "userId" });
            DropIndex("dbo.ContactUs", new[] { "user_Id" });
            DropIndex("dbo.PostImages", new[] { "PostId" });
            DropIndex("dbo.Comments", new[] { "PostId" });
            DropIndex("dbo.Qualifications", new[] { "StaffProfile_Id" });
            DropIndex("dbo.StaffProfiles", new[] { "UserId" });
            DropIndex("dbo.StudentProfiles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Subjects", new[] { "UserId" });
            DropIndex("dbo.Subjects", new[] { "ClassLevelId" });
            DropIndex("dbo.EnrolledSubjects", new[] { "EnrollmentId" });
            DropIndex("dbo.EnrolledSubjects", new[] { "SubjectId" });
            DropIndex("dbo.Enrollments", new[] { "User_Id" });
            DropIndex("dbo.Enrollments", new[] { "SessionId" });
            DropIndex("dbo.Enrollments", new[] { "StudentProfileId" });
            DropIndex("dbo.Enrollments", new[] { "ClassLevelId" });
            DropIndex("dbo.AttendanceDetails", new[] { "StudentProfile_Id" });
            DropIndex("dbo.AttendanceDetails", new[] { "AttendanceId" });
            DropIndex("dbo.AttendanceDetails", new[] { "EnrollmentId" });
            DropIndex("dbo.AttendanceDetails", new[] { "UserId" });
            DropIndex("dbo.Attendances", new[] { "SessionId" });
            DropIndex("dbo.Attendances", new[] { "EnrollmentId" });
            DropIndex("dbo.Attendances", new[] { "ClassLevelId" });
            DropIndex("dbo.Attendances", new[] { "UserId" });
            DropIndex("dbo.ClassLevels", new[] { "StaffProfile_Id" });
            DropIndex("dbo.ClassLevels", new[] { "SessionId" });
            DropIndex("dbo.ClassLevels", new[] { "UserId" });
            DropIndex("dbo.Assignments", new[] { "SubjectId" });
            DropIndex("dbo.Assignments", new[] { "SessionId" });
            DropIndex("dbo.Assignments", new[] { "ClassLevelId" });
            DropIndex("dbo.AssignmentAnswers", new[] { "StudentProfile_Id" });
            DropIndex("dbo.AssignmentAnswers", new[] { "ClassLevel_Id" });
            DropIndex("dbo.AssignmentAnswers", new[] { "EnrollementId" });
            DropIndex("dbo.AssignmentAnswers", new[] { "UserId" });
            DropIndex("dbo.AssignmentAnswers", new[] { "AssignmentId" });
            DropTable("dbo.TimeTables");
            DropTable("dbo.Syllables");
            DropTable("dbo.StudentDatas");
            DropTable("dbo.SmsReports");
            DropTable("dbo.SmsGroups");
            DropTable("dbo.Settings");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Riddles");
            DropTable("dbo.Quotes");
            DropTable("dbo.PublishResults");
            DropTable("dbo.Properties");
            DropTable("dbo.PinCodeModels");
            DropTable("dbo.States");
            DropTable("dbo.LocalGovs");
            DropTable("dbo.ImageSliders");
            DropTable("dbo.ImageModels");
            DropTable("dbo.HallOfFames");
            DropTable("dbo.Gradings");
            DropTable("dbo.GradingDetails");
            DropTable("dbo.Events");
            DropTable("dbo.Defaulters");
            DropTable("dbo.MessageReplies");
            DropTable("dbo.ContactUs");
            DropTable("dbo.PostImages");
            DropTable("dbo.Posts");
            DropTable("dbo.Comments");
            DropTable("dbo.Qualifications");
            DropTable("dbo.StaffProfiles");
            DropTable("dbo.StudentProfiles");
            DropTable("dbo.Sessions");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Subjects");
            DropTable("dbo.EnrolledSubjects");
            DropTable("dbo.Enrollments");
            DropTable("dbo.AttendanceDetails");
            DropTable("dbo.Attendances");
            DropTable("dbo.ClassLevels");
            DropTable("dbo.Assignments");
            DropTable("dbo.AssignmentAnswers");
        }
    }
}
