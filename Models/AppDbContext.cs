using Microsoft.EntityFrameworkCore;

namespace PW.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // ==========================
        // Users
        // ==========================

        public DbSet<ApplicationUser> Users { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Psychologist> Psychologists { get; set; }

        // ==========================
        // Survey
        // ==========================

        public DbSet<SurveyCategory> SurveyCategories { get; set; }

        public DbSet<Survey> Surveys { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<QuestionOption> QuestionOptions { get; set; }

        public DbSet<SurveyResult> SurveyResults { get; set; }

        public DbSet<SurveyAnswer> SurveyAnswers { get; set; }

        // ==========================
        // Appointment
        // ==========================

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<ScheduleSlot> ScheduleSlots { get; set; }

        public DbSet<PsychologicalProfile> PsychologicalProfiles { get; set; }

        // ==========================
        // Blog
        // ==========================

        public DbSet<BlogCategory> BlogCategories { get; set; }

        public DbSet<Blog> Blogs { get; set; }

        // Nếu có bình luận
        public DbSet<Comment> Comments { get; set; }
        
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // Survey Category -> Survey
            // =========================

            modelBuilder.Entity<Survey>()
                .HasOne(s => s.SurveyCategory)
                .WithMany(c => c.Surveys)
                .HasForeignKey(s => s.SurveyCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Survey -> Question
            // =========================

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Survey)
                .WithMany(s => s.Questions)
                .HasForeignKey(q => q.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // Question -> QuestionOption
            // =========================

            modelBuilder.Entity<QuestionOption>()
                .HasOne(o => o.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // Student -> SurveyResult
            // =========================

            modelBuilder.Entity<SurveyResult>()
                .HasOne(r => r.Student)
                .WithMany(s => s.SurveyResults)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SurveyResult>()
                .HasOne(r => r.Survey)
                .WithMany()
                .HasForeignKey(r => r.SurveyId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // SurveyResult -> SurveyAnswer
            // =========================

            modelBuilder.Entity<SurveyAnswer>()
                .HasOne(a => a.SurveyResult)
                .WithMany(r => r.Answers)
                .HasForeignKey(a => a.SurveyResultId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SurveyAnswer>()
                .HasOne(a => a.Question)
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SurveyAnswer>()
                .HasOne(a => a.QuestionOption)
                .WithMany()
                .HasForeignKey(a => a.QuestionOptionId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Student -> Appointment
            // =========================

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Psychologist -> Appointment
            // =========================

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Psychologist)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PsychologistId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Psychologist -> ScheduleSlot
            // =========================

            modelBuilder.Entity<ScheduleSlot>()
                .HasOne(s => s.Psychologist)
                .WithMany(p => p.ScheduleSlots)
                .HasForeignKey(s => s.PsychologistId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Student -> PsychologicalProfile
            // =========================

            modelBuilder.Entity<PsychologicalProfile>()
                .HasOne(p => p.Student)
                .WithMany(s => s.PsychologicalProfiles)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Psychologist -> PsychologicalProfile
            // =========================

            modelBuilder.Entity<PsychologicalProfile>()
                .HasOne(p => p.Psychologist)
                .WithMany(ps => ps.PsychologicalProfiles)
                .HasForeignKey(p => p.PsychologistId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // BlogCategory -> Blog
            // =========================

            modelBuilder.Entity<Blog>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Blogs)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // ApplicationUser -> Blog
            // =========================

            modelBuilder.Entity<Blog>()
                .HasOne(b => b.Author)
                .WithMany()
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Blog -> Comment
            // =========================

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Blog)
                .WithMany(b => b.Comments)
                .HasForeignKey(c => c.BlogId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // User -> Comment
            // =========================

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(s => s.Comments)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.UserId);

            modelBuilder.Entity<Psychologist>()
                .HasOne(p => p.User)
                .WithOne(u => u.Psychologist)
                .HasForeignKey<Psychologist>(p => p.UserId);
        }
    }
}