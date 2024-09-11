using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SchoolPortal.Web.Models.Entities
{

    public enum ArchiveStatus
    {

        [Description("Not Archived")]
        NotArchived = 0,
        [Description("Archived")]
        Archived = 1

    }
    public enum HomeSortFrom
    {
        [Description("Top")]
        Top = 0,

        [Description("Bottom")]
        Bottom = 2,

    }
    public enum MenuSortFrom
    {
        [Description("Left")]
        left = 0,

        [Description("Right")]
        Right = 2,

    }
    public enum UserStatus
    {
        [Description("Pending")]
        Pending = 0,

        [Description("Active")]
        Active = 2,
        [Description("Suspended")]
        Suspended = 3,

        [Description("Leave")]
        Leave = 4,
        [Description("Deleted")]
        Deleted = 6,
    }

    public enum PagePosition
    {
        [Description("None")]
        None = 0,

        [Description("Top")]
        Top = 2,

        [Description("Menu")]
        Menu = 3,
        [Description("Footer")]
        Footer = 4,
    }
    public enum AttendanceStatus
    {
        [Description("None")]
        None = 0,

        [Description("Present")]
        Present = 2,

        [Description("Absent")]
        Absent = 3,
    }
    public enum PeriodStatus
    {
        [Description("None")]
        None = 0,

        [Description("Early")]
        Early = 2,

        [Description("Ontime")]
        Ontime = 3,

        [Description("Late")]
        Late = 5,

        [Description("VeryLate")]
        VeryLate = 7
    }
    public enum TrainingStatus
    {
        [Description("None")]
        None = 0,

        [Description("Done")]
        Done = 2,

        [Description("Canceled")]
        Canceled = 3,
        [Description("Missed")]
        Missed = 8,

    }
    public enum FundStatus
    {
        [Description("Success")]
        Success = 2,

        [Description("Failed")]
        Failed = 3,
        [Description("Pending")]
        Pending = 4,

    }

    public enum ProposalStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Approved")]
        Approved = 2,

        [Description("Failed")]
        Failed = 3,

        [Description("Submitted")]
        Submitted = 6
    }
    public enum FundType
    {
        [Description("Credit")]
        Credit = 2,

        [Description("Debit")]
        Debit = 3,

    }
    public enum FundMeans
    {
        [Description("Transfer")]
        Transfer = 2,

        [Description("Cash")]
        Cash = 3,
        [Description("Check")]
        Check = 4,
    }
    public enum ReportPeriodStatus
    {
        [Description("None")]
        None = 0,

        [Description("Early")]
        Early = 2,

        [Description("Ontime")]
        Ontime = 3,

        [Description("Late")]
        Late = 5,

        [Description("VeryLate")]
        VeryLate = 7
    }
    public enum ReportStatus
    {
        [Description("None")]
        None = 0,

        [Description("Submitted")]
        Submitted = 2,
        [Description("Failed")]
        Failed = 3,
        [Description("NoReport")]
        NoReport = 4,
    }       public enum ModalTime
        {
            [Description("None")]
            None = 0,

            [Description("Daily")]
            Daily = 2,

            [Description("Weekly")]
            Weekly = 3,

            [Description("Hourly")]
            Hourly = 4,

        }
        public enum ModalOccurance
        {
            [Description("None")]
            None = 0,

            [Description("Once")]
            Once = 2,

            [Description("Repeated")]
            Repeated = 3,

        }
    public enum VerificationStatus
    {
        [Description("Pending")]
        Pending = 0,

        [Description("Verified")]
        Verified = 2,

    }
    public enum TaskNoteStatus
    {
        [Description("None")]
        None = 0,

        [Description("Active")]
        Active = 2,

    }
    public enum LiveStatus
    {

        [Description("Active")]
        Active = 1,
        [Description("Ended")]
        Ended = 2,
        [Description("Waiting")]
        Waiting = 3,

    }

    public enum UploadType
    {
        [Description("Word")]
        [Display(Name = "Word")]
        Word = 1,

        [Description("Excel")]
        [Display(Name = "Excel")]
        Excel = 2,

        [Description("Powerpoint")]
        [Display(Name = "Powerpoint")]
        Powerpoint = 3,

        [Description("PDF")]
        [Display(Name = "PDF")]
        PDF = 4,

        [Description("Video")]
        [Display(Name = "Video")]
        Video = 5,

        [Description("Audio")]
        [Display(Name = "Audio")]
        Audio = 6,


    }
    public enum Mode
    {
        [Description("None")]
        None = 0,

        [Description("School Exam")]
        Exam = 9,

        [Description("School Test")]
        Test = 4,

        [Description("Practice")]
        Practice = 1,

        [Description("Study")]
        Study = 2,

        [Description("Mock")]
        Mock = 3,

        [Description("JAMB")]
        JAMB = 5,

        [Description("WAEC")]
        WAEC = 6,

        [Description("NECO")]
        NECO = 7,

        [Description("GCE")]
        GCE = 8




    }

    public enum GradingOption
    {
        [Description("NONE")]
        NONE = 0,

        [Description("JSS")]
        JSS = 1,

        [Description("SSS")]
        SSS = 2,

        [Description("PRI")]
        PRI = 3,

        [Description("NUR")]
        NUR = 4,


        [Description("PG")]
        PG = 5,
        [Description("PRE")]
        PRE = 6,


    }



    public enum AssignmentType
    {
        [Description("NONE")]
        NONE = 0,

        [Description("Mid Term")]
        MidTerm = 1,

        [Description("Holiday")]
        Holiday = 2,


    }

    public enum EntityStatus
    {
        [Description("None")]
        None = 0,
        [Description("Active")]
        Active = 1,
        [Description("Not Active")]
        NotActive = 6,
        [Description("Expelled")]
        Expelled = 2,
        [Description("Withdrawn")]
        Withdrawn = 3,
        [Description("Archived")]
        Archived = 4,
        [Description("Suspended")]
        Suspeneded = 5,

        [Description("Graduate")]
        Graduate = 7,
        [Description("Dropout")]
        Dropout = 9


    }

    public enum MailStatus
    {
        Sent = 1,

        SavedAsDraft = 2,

        Archived = 3
    }


    public enum PostStatus
    {
        [Description("Published")]
        Published = 1,

        [Description("Deleted")]
        Deleted = 2
    }
    public enum FinanceType
    {
        [Description("Debit")]
        Debit = 1,

        [Description("Credit")]
        Credit = 2
    }
    public enum TransactionStatus
    {
        [Description("Paid")]
        Paid = 1,

        [Description("Pending")]
        Pending = 2,

        [Description("Cancel")]
        Cancel = 3,

        [Description("Part")]
        Part = 4,
        [Description("CompletePart")]
        CompletePart = 5
    }
    public enum FinanceSource
    {
        [Description("Cash")]
        Cash = 1,
        [Description("Bank")]
        Bank = 2,
        [Description("Online")]
        Online = 3
    }

    public enum PaymentStatus
    {
        [Description("Paid")]
        Paid = 1,

        [Description("Unpaid")]
        UnPaid = 2,

        [Description("Failed")]
        Failed = 3

    }

    public enum WhoSeePost
    {
        [Description("All")]
        All = 0,
        [Description("Student")]
        Student = 1,

        [Description("Staff")]
        Staff = 2
    }

    public enum PageType
    {
        [Description("Menu")]
        Menu = 1,

        [Description("Body")]
        Body = 2,

        [Description("Article")]
        Article = 3,

        [Description("News")]
        News = 4,

        [Description("About Us")]
        AboutUs = 5,

        [Description("Event")]
        Event = 6
    }

    public enum SessionStatus
    {
        [Description("Not Used")]
        NotUsed = 1,

        [Description("Used")]
        Used = 2,

        [Description("Current")]
        Current = 3
    }

    public enum RequiresPass
    {
        [Description("No")]
        No = 1,

        [Description("Yes")]
        Yes = 2
    }

    public enum SubjectStatus
    {
        [Description("Active")]
        Active = 1,

        [Description("Inactive")]
        Inactive = 2
    }
    public enum MessageStatus
    {
        [Description("NotReplied")]
        NotReplied = 1,

        [Description("Replied")]
        Replied = 2,
        [Description("Closed")]
        Closed = 3
    }

    public enum AdmissionStatus
    {
        [Description("Complete Registration")]
        CompleteRegistration = 1,

        [Description("Given Admission")]
        GivenAdmission = 2,
        [Description("Not Given Admission")]
        NotGivenAdmission = 3,
        [Description("Disqualified")]
        Disqualified = 4
    }

    public enum PrintOutOption
    {
        [Description("Result Print Out One")]
        PrintOutOne = 1,

        [Description("Result Print Out Two")]
        PrintOutTwo = 2,

        [Description("Result Print Out Three")]
        PrintOutThree = 3
    }

    public enum MenuDescription
    {
        [Description("None")]
        None = 0,

        [Description("Dropdown Menu")]
        Dropdown = 1,

        [Description("Main Menu")]
        MainMenu = 2,

        [Description("Home Body")]
        HomeBody = 3,

        [Description("Above Footer")]
        AboveFooter = 4,

        [Description("Footer")]
        Footer = 5,

        [Description("Detail Page Crumb")]
        DetailPageCrumb = 6,

        [Description("Menu Top Header")]
        MenuTopHeader = 7,

        [Description("Head Tag Content")]
        HeadTagContent = 8,

        [Description("Head Stylesheet")]
        HeadStylesheet = 9,

        [Description("Override CSS")]
        OverrideCSS = 10,

    }

    public enum HelpType
    {
        [Description("Document")]
        Document = 1,

        [Description("Video")]
        Video = 2
    }
    public enum TicketPriority
    {
        [Description("Normal")]
        Normal = 1,

        [Description("Emergency")]
        Emergency = 2,
        [Description("High")]
        High = 3,
        [Description("Low")]
        Low = 4
    }

    public enum PagePublish
    {
        [Description("Not Publish")]
        NotPublish = 0,

        [Description("Publish")]
        Publish = 1

    }


    public enum AdmissionPinOption
    {
        [Description("Used Pin In Registration")]
        UsedPin = 1,

        [Description("Don't Pin in Registration")]
        NoPin = 2
    }


    public enum PinValidOption
    {
        [Description("Termly")]
        Termly = 1,

        [Description("Yearly")]
        Yearly = 2
    }

    public enum HostelStatus
    {
        [Description("Free")]
        free = 0,
        [Description("Free")]
        Free = 2,

        [Description("Full")]
        Full = 2
    }

    public enum HostelRoomStatus
    {
        [Description("Free")]
        free = 0,
        [Description("Free")]
        Free = 1,

        [Description("Full")]
        Full = 2
    }

    public enum AllocationStatus
    {
        [Description("Allocated")]
        Allocated = 1,

        [Description("Not Allocated")]
        NotAllocated = 2
    }

    public enum HostelBedStatus
    {
        [Description("Free")]
        free = 0,
        [Description("Free")]
        Free = 1,

        [Description("Taken")]
        Taken = 2
    }

    public enum ExamStatus
    {
        [Description("Not Started")]
        NotStarted = 0,

        [Description("Started")]
        Started = 1,

        [Description("Completed")]
        Completed = 2,

    }
}