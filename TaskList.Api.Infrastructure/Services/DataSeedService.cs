using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskList.Api.Domain.Tasks.Enums;
using TaskList.Api.Domain.Tasks.Models;
using TaskList.Api.Domain.Users.Models.AuthenticationModels;
using TaskList.Api.Infrastructure.Data;

namespace TaskList.Api.Infrastructure.Services
{
    public class DataSeedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DataSeedService> _logger;

        public DataSeedService(IServiceProvider serviceProvider, ILogger<DataSeedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskListDbContext>();

            await SeedUserAliAsync(userManager, dbContext, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task SeedUserAliAsync(UserManager<ApplicationUser> userManager, TaskListDbContext dbContext, CancellationToken cancellationToken)
        {
            const string userName = "ALiDoe999";
            const string email = "Ali@Imparta.com";
            const string displayName = "Ali";
            const string password = "Password12345!";

            try
            {
                // Check if user already exists
                var existingUser = await userManager.FindByNameAsync(userName);
                if (existingUser != null)
                {
                    _logger.LogInformation("User {UserName} already exists. Skipping seed.", userName);
                    return;
                }

                // Create the user
                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    DisplayName = displayName,
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedDateUtc = DateTime.UtcNow,
                    HasProfilePicture = false
                };

                var result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to create user {UserName}. Errors: {Errors}", 
                        userName, string.Join(", ", result.Errors.Select(e => e.Description)));
                    return;
                }

                _logger.LogInformation("Successfully created user {UserName}", userName);

                // Get the created user with ID
                var createdUser = await userManager.FindByNameAsync(userName);
                if (createdUser == null)
                {
                    _logger.LogError("Failed to retrieve created user {UserName}", userName);
                    return;
                }

                // Create first ToDoList: "Jobs designated by my Boss"
                var bossList = new ToDoList
                {
                    Title = "Jobs designated by my Boss",
                    Description = "Tasks and assignments that have been delegated to me by my supervisor for completion.",
                    UserId = createdUser.Id
                };

                await dbContext.ToDoLists.AddAsync(bossList, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                // Create second ToDoList: "Jobs from Client"
                var clientList = new ToDoList
                {
                    Title = "Jobs from Client",
                    Description = "Client requests and deliverables that need to be completed as per project requirements.",
                    UserId = createdUser.Id
                };

                await dbContext.ToDoLists.AddAsync(clientList, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Created 2 ToDoLists for user {UserName}", userName);

                // Seed tasks for "Jobs designated by my Boss"
                await SeedBossTasksAsync(dbContext, bossList.Id, cancellationToken);

                // Seed tasks for "Jobs from Client"
                await SeedClientTasksAsync(dbContext, clientList.Id, cancellationToken);

                _logger.LogInformation("Successfully seeded all data for user {UserName}", userName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding data for user {UserName}", userName);
                throw;
            }
        }

        private async Task SeedBossTasksAsync(TaskListDbContext dbContext, int toDoListId, CancellationToken cancellationToken)
        {
            var tasks = new List<ToDoItem>
            {
                new ToDoItem
                {
                    Title = "Prepare quarterly performance report",
                    Description = "Compile and analyze team performance metrics for Q4 2024. Include detailed charts, trends analysis, and recommendations for improvement based on key performance indicators.",
                    Type = "Report",
                    Status = ProgressStatus.Done,
                    Priority = PriorityStatus.High,
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    CompletedAt = DateTime.UtcNow.AddDays(-2),
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Review budget proposals for next fiscal year",
                    Description = "Analyze departmental budget requests and provide feedback on resource allocation. Ensure alignment with company strategic goals and identify cost-saving opportunities.",
                    Type = "Review",
                    Status = ProgressStatus.InProgress,
                    Priority = PriorityStatus.High,
                    CreatedAt = DateTime.UtcNow.AddDays(-8),
                    CompletedAt = null,
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Conduct team member performance evaluations",
                    Description = "Complete mid-year performance reviews for all direct reports. Prepare constructive feedback, set development goals, and schedule one-on-one meetings with each team member.",
                    Type = "Administrative",
                    Status = ProgressStatus.NotStarted,
                    Priority = PriorityStatus.Medium,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    CompletedAt = null,
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Update employee handbook with new policies",
                    Description = "Revise the employee handbook to include recently approved remote work policies and updated benefits information. Coordinate with HR for legal compliance review.",
                    Type = "Documentation",
                    Status = ProgressStatus.Done,
                    Priority = PriorityStatus.Low,
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    CompletedAt = DateTime.UtcNow.AddDays(-10),
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Organize leadership training workshop",
                    Description = "Plan and coordinate a two-day leadership development workshop for senior team members. Book venue, arrange catering, prepare materials, and confirm external facilitator availability.",
                    Type = "Event Planning",
                    Status = ProgressStatus.InProgress,
                    Priority = PriorityStatus.High,
                    CreatedAt = DateTime.UtcNow.AddDays(-12),
                    CompletedAt = null,
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Audit current project timelines",
                    Description = "Review all active projects to ensure they are on track with established deadlines. Identify potential delays and escalate issues that require management intervention or additional resources.",
                    Type = "Audit",
                    Status = ProgressStatus.NotStarted,
                    Priority = PriorityStatus.Medium,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    CompletedAt = null,
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Draft new customer service protocol",
                    Description = "Develop comprehensive guidelines for handling customer complaints and service requests. Include escalation procedures, response time standards, and quality assurance checkpoints for team implementation.",
                    Type = "Documentation",
                    Status = ProgressStatus.InProgress,
                    Priority = PriorityStatus.Medium,
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    CompletedAt = null,
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Coordinate IT system upgrade",
                    Description = "Work with IT department to schedule and implement company-wide software system upgrade. Ensure minimal disruption to operations and arrange training sessions for affected staff members.",
                    Type = "Technical",
                    Status = ProgressStatus.Done,
                    Priority = PriorityStatus.High,
                    CreatedAt = DateTime.UtcNow.AddDays(-25),
                    CompletedAt = DateTime.UtcNow.AddDays(-18),
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Research competitor market strategies",
                    Description = "Conduct comprehensive market analysis of top three competitors' recent marketing campaigns and product launches. Prepare strategic recommendations for our positioning and differentiation approach.",
                    Type = "Research",
                    Status = ProgressStatus.NotStarted,
                    Priority = PriorityStatus.Low,
                    CreatedAt = DateTime.UtcNow.AddDays(-4),
                    CompletedAt = null,
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Schedule quarterly board meeting",
                    Description = "Coordinate with executive team and board members to find suitable date for Q1 board meeting. Prepare preliminary agenda, book conference room, and arrange virtual attendance options for remote participants.",
                    Type = "Administrative",
                    Status = ProgressStatus.InProgress,
                    Priority = PriorityStatus.High,
                    CreatedAt = DateTime.UtcNow.AddDays(-6),
                    CompletedAt = null,
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Implement new onboarding process",
                    Description = "Roll out the revised employee onboarding program including updated orientation materials, mentor assignment system, and 90-day check-in procedures for all new hires starting next month.",
                    Type = "Process Improvement",
                    Status = ProgressStatus.NotStarted,
                    Priority = PriorityStatus.Medium,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    CompletedAt = null,
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Review and approve vendor contracts",
                    Description = "Examine proposed contracts from three potential vendors for office supplies and equipment. Compare pricing, terms, and service levels to make informed procurement recommendations.",
                    Type = "Review",
                    Status = ProgressStatus.Done,
                    Priority = PriorityStatus.Medium,
                    CreatedAt = DateTime.UtcNow.AddDays(-14),
                    CompletedAt = DateTime.UtcNow.AddDays(-5),
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Create department goals presentation",
                    Description = "Develop a comprehensive PowerPoint presentation outlining departmental objectives, key results, and success metrics for the upcoming quarter. Include visual data representations and action plans for team review.",
                    Type = "Presentation",
                    Status = ProgressStatus.InProgress,
                    Priority = PriorityStatus.High,
                    CreatedAt = DateTime.UtcNow.AddDays(-9),
                    CompletedAt = null,
                    ToDoListId = toDoListId
                }
            };

            await dbContext.ToDoItems.AddRangeAsync(tasks, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedClientTasksAsync(TaskListDbContext dbContext, int toDoListId, CancellationToken cancellationToken)
        {
            var tasks = new List<ToDoItem>
            {
                new ToDoItem
                {
                    Title = "Design new website landing page",
                    Description = "Create a modern, responsive landing page design for client's product launch campaign. Incorporate brand guidelines, optimize for conversion, and ensure mobile-friendly layout with compelling call-to-action elements.",
                    Type = "Design",
                    Status = ProgressStatus.Done,
                    Priority = PriorityStatus.High,
                    CreatedAt = DateTime.UtcNow.AddDays(-18),
                    CompletedAt = DateTime.UtcNow.AddDays(-7),
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Develop custom API integration",
                    Description = "Build secure API endpoints to integrate client's CRM system with third-party payment gateway. Implement proper authentication, error handling, and comprehensive logging for transaction tracking and troubleshooting.",
                    Type = "Development",
                    Status = ProgressStatus.InProgress,
                    Priority = PriorityStatus.High,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    CompletedAt = null,
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Write technical documentation for software release",
                    Description = "Prepare detailed user guides, API documentation, and troubleshooting resources for version 2.0 release. Include screenshots, code examples, and step-by-step instructions for end users and developers.",
                    Type = "Documentation",
                    Status = ProgressStatus.NotStarted,
                    Priority = PriorityStatus.Medium,
                    CreatedAt = DateTime.UtcNow.AddDays(-4),
                    CompletedAt = null,
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Conduct user acceptance testing",
                    Description = "Coordinate UAT sessions with client stakeholders to validate new features and functionality. Document feedback, track issues in project management system, and ensure all acceptance criteria are met before production deployment.",
                    Type = "Testing",
                    Status = ProgressStatus.InProgress,
                    Priority = PriorityStatus.High,
                    CreatedAt = DateTime.UtcNow.AddDays(-8),
                    CompletedAt = null,
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Create social media marketing campaign",
                    Description = "Develop comprehensive social media strategy for Q1 product promotion including content calendar, graphics, copywriting, and platform-specific targeting. Align messaging with overall brand positioning and campaign objectives.",
                    Type = "Marketing",
                    Status = ProgressStatus.Done,
                    Priority = PriorityStatus.Medium,
                    CreatedAt = DateTime.UtcNow.AddDays(-22),
                    CompletedAt = DateTime.UtcNow.AddDays(-12),
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Optimize database performance",
                    Description = "Analyze current database queries and implement indexing strategies to improve response times. Identify and resolve bottlenecks, optimize slow queries, and implement caching mechanisms for frequently accessed data.",
                    Type = "Technical",
                    Status = ProgressStatus.InProgress,
                    Priority = PriorityStatus.High,
                    CreatedAt = DateTime.UtcNow.AddDays(-6),
                    CompletedAt = null,
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Design mobile app user interface",
                    Description = "Create intuitive and visually appealing UI mockups for iOS and Android applications. Follow material design principles, ensure accessibility compliance, and incorporate user feedback from previous design iterations.",
                    Type = "Design",
                    Status = ProgressStatus.NotStarted,
                    Priority = PriorityStatus.Medium,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    CompletedAt = null,
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Prepare project status report for stakeholders",
                    Description = "Compile comprehensive progress update covering completed milestones, current sprint activities, upcoming deliverables, budget status, and risk assessment. Present clear metrics and timeline projections for executive review.",
                    Type = "Report",
                    Status = ProgressStatus.Done,
                    Priority = PriorityStatus.High,
                    CreatedAt = DateTime.UtcNow.AddDays(-16),
                    CompletedAt = DateTime.UtcNow.AddDays(-9),
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Implement security audit recommendations",
                    Description = "Address vulnerabilities identified in recent security assessment including updating dependencies, implementing additional authentication measures, and enhancing data encryption protocols. Prioritize critical and high-severity findings.",
                    Type = "Security",
                    Status = ProgressStatus.InProgress,
                    Priority = PriorityStatus.High,
                    CreatedAt = DateTime.UtcNow.AddDays(-11),
                    CompletedAt = null,
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Conduct client training session",
                    Description = "Deliver hands-on training workshop for client's administrative team on new system features and workflows. Prepare training materials, create practice scenarios, and provide post-session support documentation.",
                    Type = "Training",
                    Status = ProgressStatus.NotStarted,
                    Priority = PriorityStatus.Low,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    CompletedAt = null,
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Migrate data to new platform",
                    Description = "Plan and execute secure data migration from legacy system to modern cloud infrastructure. Validate data integrity, maintain audit trails, and ensure zero downtime during transition with proper rollback procedures.",
                    Type = "Migration",
                    Status = ProgressStatus.Done,
                    Priority = PriorityStatus.High,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    CompletedAt = DateTime.UtcNow.AddDays(-20),
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Build automated testing suite",
                    Description = "Develop comprehensive automated test suite covering unit tests, integration tests, and end-to-end scenarios. Implement continuous integration pipeline to run tests on every code commit.",
                    Type = "Development",
                    Status = ProgressStatus.InProgress,
                    Priority = PriorityStatus.Medium,
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    CompletedAt = null,
                    ToDoListId = toDoListId
                },
                new ToDoItem
                {
                    Title = "Create brand identity guidelines",
                    Description = "Develop comprehensive brand style guide including logo usage, color palettes, typography standards, imagery guidelines, and tone of voice specifications. Ensure consistency across all client communication channels.",
                    Type = "Design",
                    Status = ProgressStatus.NotStarted,
                    Priority = PriorityStatus.Low,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    CompletedAt = null,
                    ToDoListId = toDoListId
                }
            };

            await dbContext.ToDoItems.AddRangeAsync(tasks, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
