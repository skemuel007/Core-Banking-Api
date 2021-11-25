using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AliasWebApiCore.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommonSequences",
                columns: table => new
                {
                    CommonSequenceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Counter = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    FixedLength = table.Column<int>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Prefix = table.Column<string>(nullable: true),
                    Suffix = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonSequences", x => x.CommonSequenceId);
                });

            migrationBuilder.CreateTable(
                name: "CustomerTypes",
                columns: table => new
                {
                    CustomerTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    Type = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerTypes", x => x.CustomerTypeId);
                });

            migrationBuilder.CreateTable(
                name: "FileUploads",
                columns: table => new
                {
                    FileId = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    FileLength = table.Column<long>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    FilePath = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileUploads", x => x.FileId);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    GroupId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    LoanId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.LoanId);
                });

            migrationBuilder.CreateTable(
                name: "MainGeneralLedgerCodes",
                columns: table => new
                {
                    MainGeneralLedgerCodeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 20, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(maxLength: 100, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    Status = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainGeneralLedgerCodes", x => x.MainGeneralLedgerCodeId);
                });

            migrationBuilder.CreateTable(
                name: "SmsConfig",
                columns: table => new
                {
                    SmsConfigId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    HostUrl = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    OptKey = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Sender = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsConfig", x => x.SmsConfigId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BranchId = table.Column<int>(nullable: true),
                    ChatName = table.Column<string>(maxLength: 50, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Enabled = table.Column<bool>(nullable: true),
                    FirstName = table.Column<string>(maxLength: 150, nullable: false),
                    ImageFile = table.Column<string>(maxLength: 100, nullable: true),
                    LastName = table.Column<string>(maxLength: 150, nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    OtherNames = table.Column<string>(maxLength: 150, nullable: true),
                    PasswordChangeFreq = table.Column<string>(nullable: true),
                    Reset = table.Column<int>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    LastLogOut = table.Column<DateTime>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LoginTime = table.Column<DateTime>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    isLoggedIn = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BankDetails",
                columns: table => new
                {
                    BankDetailsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActFile = table.Column<string>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    DatabaseId = table.Column<string>(nullable: true),
                    LocationAddress = table.Column<string>(nullable: true),
                    Logo = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    PostalAddress = table.Column<string>(nullable: true),
                    ServerMacId = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankDetails", x => x.BankDetailsId);
                    table.ForeignKey(
                        name: "FK_BankDetails_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BranchDetails",
                columns: table => new
                {
                    BranchId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountFile = table.Column<string>(nullable: true),
                    Area = table.Column<string>(nullable: true),
                    BranchCode = table.Column<string>(nullable: true),
                    BranchName = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    Databasefile = table.Column<string>(nullable: true),
                    District = table.Column<string>(nullable: true),
                    LocationAddress = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    PostalAddress = table.Column<string>(nullable: true),
                    Region = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    TelephoneNumber = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchDetails", x => x.BranchId);
                    table.ForeignKey(
                        name: "FK_BranchDetails_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CountryLists",
                columns: table => new
                {
                    CountryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    Desc = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryLists", x => x.CountryId);
                    table.ForeignKey(
                        name: "FK_CountryLists_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GeneralLedgerCodes",
                columns: table => new
                {
                    GeneralLedgerCodeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AllowJournals = table.Column<bool>(nullable: false),
                    BalanceType = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    GLType = table.Column<string>(nullable: true),
                    MainGeneralLedgerCodeId = table.Column<int>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    SubCode = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralLedgerCodes", x => x.GeneralLedgerCodeId);
                    table.ForeignKey(
                        name: "FK_GeneralLedgerCodes_MainGeneralLedgerCodes_MainGeneralLedgerCodeId",
                        column: x => x.MainGeneralLedgerCodeId,
                        principalTable: "MainGeneralLedgerCodes",
                        principalColumn: "MainGeneralLedgerCodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeneralLedgerCodes_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JointCustomers",
                columns: table => new
                {
                    JointId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    GeneralBroadAlert = table.Column<bool>(nullable: false),
                    JointNumber = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    SupportAlert = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JointCustomers", x => x.JointId);
                    table.ForeignKey(
                        name: "FK_JointCustomers_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ledgers",
                columns: table => new
                {
                    LedgerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ledgers", x => x.LedgerId);
                    table.ForeignKey(
                        name: "FK_Ledgers_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    RatingId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.RatingId);
                    table.ForeignKey(
                        name: "FK_Ratings_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sectors",
                columns: table => new
                {
                    SectorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectors", x => x.SectorId);
                    table.ForeignKey(
                        name: "FK_Sectors_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Targets",
                columns: table => new
                {
                    TargetId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Targets", x => x.TargetId);
                    table.ForeignKey(
                        name: "FK_Targets_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new
                {
                    CreatedUserId = table.Column<int>(nullable: false),
                    GroupId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => new { x.CreatedUserId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_UserGroups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserGroups_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalRules",
                columns: table => new
                {
                    ApprovalRulesId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApproversUserIds = table.Column<string>(nullable: true),
                    BranchId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false),
                    MaximumAmount = table.Column<decimal>(type: "Money", nullable: false),
                    MinimumAmount = table.Column<decimal>(type: "Money", nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRules", x => x.ApprovalRulesId);
                    table.ForeignKey(
                        name: "FK_ApprovalRules_BranchDetails_BranchId",
                        column: x => x.BranchId,
                        principalTable: "BranchDetails",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceConfig",
                columns: table => new
                {
                    ServiceConfigId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BranchId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    CronSchedule = table.Column<string>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Frequency = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    ServiceType = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    StartUpType = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceConfig", x => x.ServiceConfigId);
                    table.ForeignKey(
                        name: "FK_ServiceConfig_BranchDetails_BranchId",
                        column: x => x.BranchId,
                        principalTable: "BranchDetails",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SessionManager",
                columns: table => new
                {
                    SessionManagerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BranchId = table.Column<int>(nullable: false),
                    ClosedDate = table.Column<DateTime>(nullable: true),
                    ClosedUserId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    SessionDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionManager", x => x.SessionManagerId);
                    table.ForeignKey(
                        name: "FK_SessionManager_BranchDetails_BranchId",
                        column: x => x.BranchId,
                        principalTable: "BranchDetails",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sweeps",
                columns: table => new
                {
                    SweepId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<decimal>(type: "Money", nullable: false),
                    ApprovedDate = table.Column<DateTime>(nullable: true),
                    ApprovedNote = table.Column<string>(nullable: true),
                    ApprovedUserId = table.Column<int>(nullable: true),
                    AutoRelease = table.Column<bool>(nullable: true),
                    BranchId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Frequency = table.Column<string>(nullable: true),
                    FromAccountId = table.Column<int>(nullable: false),
                    MinimumBalance = table.Column<decimal>(type: "Money", nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    Reference = table.Column<string>(nullable: true),
                    RejectedDate = table.Column<DateTime>(nullable: true),
                    RejectedNote = table.Column<string>(nullable: true),
                    RejectedUserId = table.Column<int>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    ToAccountId = table.Column<int>(nullable: false),
                    TransferDay = table.Column<int>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sweeps", x => x.SweepId);
                    table.ForeignKey(
                        name: "FK_Sweeps_BranchDetails_BranchId",
                        column: x => x.BranchId,
                        principalTable: "BranchDetails",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Corporates",
                columns: table => new
                {
                    CorporateCustId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    BroadcastAlert = table.Column<bool>(nullable: false),
                    ComapnyLocation = table.Column<string>(nullable: true),
                    CompanyEmail = table.Column<string>(nullable: true),
                    CompanyFax = table.Column<string>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true),
                    CompanyPhone = table.Column<string>(nullable: true),
                    CompanyWebsite = table.Column<string>(nullable: true),
                    CorporateNumber = table.Column<string>(nullable: true),
                    CountryListCountryId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    DateOfIncorporation = table.Column<DateTime>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    NatureOfBusiness = table.Column<string>(nullable: true),
                    Placement = table.Column<bool>(nullable: false),
                    RegistrationNumber = table.Column<string>(nullable: true),
                    RelationalOfficer = table.Column<int>(nullable: true),
                    SupportAlert = table.Column<bool>(nullable: false),
                    TinNumber = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Corporates", x => x.CorporateCustId);
                    table.ForeignKey(
                        name: "FK_Corporates_CountryLists_CountryListCountryId",
                        column: x => x.CountryListCountryId,
                        principalTable: "CountryLists",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Corporates_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Banktiers",
                columns: table => new
                {
                    BanktiersId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChargeXAmountForActivityPeriod = table.Column<decimal>(nullable: false),
                    ChargeXAmountForActivityPeriodDate = table.Column<DateTime>(nullable: false),
                    ConsiderOverdraftForPeriod = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FlatRate = table.Column<bool>(nullable: false),
                    FlatRateAmount = table.Column<decimal>(type: "Money", nullable: false),
                    GeneralLedgerCodeId = table.Column<int>(nullable: false),
                    MaximumAmount = table.Column<decimal>(type: "Money", nullable: false),
                    MinimumAmount = table.Column<decimal>(type: "Money", nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Percentage = table.Column<bool>(nullable: false),
                    PercentageValue = table.Column<double>(nullable: false),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banktiers", x => x.BanktiersId);
                    table.ForeignKey(
                        name: "FK_Banktiers_GeneralLedgerCodes_GeneralLedgerCodeId",
                        column: x => x.GeneralLedgerCodeId,
                        principalTable: "GeneralLedgerCodes",
                        principalColumn: "GeneralLedgerCodeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Teller",
                columns: table => new
                {
                    TellerID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BranchId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    DepositLimit = table.Column<decimal>(type: "Money", nullable: false),
                    GeneralLedgerCodeId = table.Column<int>(nullable: false),
                    LastLoginDate = table.Column<DateTime>(nullable: true),
                    LastLogoutDate = table.Column<DateTime>(nullable: true),
                    LoginStatus = table.Column<bool>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    Pin = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    WithdrawalLimit = table.Column<decimal>(type: "Money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teller", x => x.TellerID);
                    table.ForeignKey(
                        name: "FK_Teller_BranchDetails_BranchId",
                        column: x => x.BranchId,
                        principalTable: "BranchDetails",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Teller_GeneralLedgerCodes_GeneralLedgerCodeId",
                        column: x => x.GeneralLedgerCodeId,
                        principalTable: "GeneralLedgerCodes",
                        principalColumn: "GeneralLedgerCodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Teller_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Individuals",
                columns: table => new
                {
                    IndividualCustId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    AddressVerified = table.Column<string>(nullable: true),
                    BiometricIdNumber = table.Column<string>(nullable: true),
                    BroadcastAlert = table.Column<bool>(nullable: false),
                    City = table.Column<string>(nullable: true),
                    CountryId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    CustomerNumber = table.Column<string>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    FirstAccOfficerId = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    Gender = table.Column<string>(nullable: false),
                    HomeType = table.Column<string>(nullable: true),
                    HouseNumber = table.Column<string>(nullable: true),
                    IdDateExpire = table.Column<DateTime>(nullable: false),
                    IdDateIssued = table.Column<DateTime>(nullable: false),
                    IdNumber = table.Column<string>(nullable: true),
                    IdVerified = table.Column<string>(nullable: true),
                    KAddress = table.Column<string>(nullable: true),
                    KEmail = table.Column<string>(nullable: true),
                    KName = table.Column<string>(nullable: true),
                    KPhone = table.Column<string>(nullable: true),
                    KRelation = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: false),
                    MaritalStatus = table.Column<string>(nullable: true),
                    Mobile = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    MotherDateOfBirth = table.Column<DateTime>(nullable: false),
                    MotherFirstName = table.Column<string>(nullable: true),
                    MotherMaidenName = table.Column<string>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    NumberOfChildren = table.Column<int>(nullable: true),
                    Occupation = table.Column<string>(nullable: true),
                    OtherContact = table.Column<string>(nullable: true),
                    OtherName = table.Column<string>(nullable: false),
                    Picture = table.Column<string>(nullable: true),
                    PostCode = table.Column<int>(nullable: false),
                    PostalAddress = table.Column<string>(nullable: true),
                    RatingId = table.Column<int>(nullable: false),
                    RelatedDocumentId = table.Column<int>(nullable: true),
                    SSNumber = table.Column<string>(nullable: true),
                    SectorId = table.Column<int>(nullable: false),
                    SecurityGroup = table.Column<string>(nullable: true),
                    Signature = table.Column<string>(nullable: true),
                    SpouseDateOfBirth = table.Column<DateTime>(nullable: false),
                    SpouseName = table.Column<string>(nullable: true),
                    SpouseOtherName = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    SupportAlert = table.Column<bool>(nullable: false),
                    TargetId = table.Column<int>(nullable: false),
                    Telephone = table.Column<string>(nullable: true),
                    TinNumber = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Individuals", x => x.IndividualCustId);
                    table.ForeignKey(
                        name: "FK_Individuals_CountryLists_CountryId",
                        column: x => x.CountryId,
                        principalTable: "CountryLists",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Individuals_Ratings_RatingId",
                        column: x => x.RatingId,
                        principalTable: "Ratings",
                        principalColumn: "RatingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Individuals_Sectors_SectorId",
                        column: x => x.SectorId,
                        principalTable: "Sectors",
                        principalColumn: "SectorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Individuals_Targets_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Targets",
                        principalColumn: "TargetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Individuals_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LoanServicings",
                columns: table => new
                {
                    LoanServicingCustId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    AddressVerified = table.Column<string>(nullable: true),
                    AverageSalary = table.Column<string>(nullable: true),
                    Banker = table.Column<string>(nullable: true),
                    BiometricIdNumber = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    CountryId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    CurrentPosition = table.Column<string>(nullable: true),
                    CurrentStation = table.Column<string>(nullable: true),
                    CustomerNumber = table.Column<string>(nullable: true),
                    CustomerTypeId = table.Column<int>(nullable: false),
                    DateEmployed = table.Column<DateTime>(nullable: false),
                    DateOfBirth = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    FirstAccOfficerId = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    Gender = table.Column<string>(nullable: false),
                    GeneralBroadAlert = table.Column<bool>(nullable: false),
                    HomeType = table.Column<string>(nullable: true),
                    HouseNumber = table.Column<string>(nullable: true),
                    IdDateExpire = table.Column<DateTime>(nullable: true),
                    IdDateIssued = table.Column<DateTime>(nullable: true),
                    IdVerified = table.Column<string>(nullable: true),
                    InvestmentAlert = table.Column<bool>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    LoanPaymentAlert = table.Column<bool>(nullable: false),
                    MaritalStatus = table.Column<string>(nullable: true),
                    Mobile = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    MotherDateOfBirth = table.Column<DateTime>(nullable: false),
                    MotherFirstName = table.Column<string>(nullable: true),
                    MotherMaidenName = table.Column<string>(nullable: true),
                    NameOfEmployer = table.Column<string>(nullable: true),
                    NumberOfChildren = table.Column<int>(nullable: true),
                    Occupation = table.Column<int>(nullable: false),
                    OtherName = table.Column<string>(nullable: false),
                    Picture = table.Column<string>(nullable: true),
                    PostalAddress = table.Column<string>(nullable: true),
                    RatingId = table.Column<int>(nullable: false),
                    RelatedDocumentId = table.Column<int>(nullable: true),
                    SSNumber = table.Column<string>(nullable: true),
                    SecondAccOfficerId = table.Column<int>(nullable: false),
                    SectorId = table.Column<int>(nullable: false),
                    SecurityGroup = table.Column<string>(nullable: true),
                    Signature = table.Column<string>(nullable: true),
                    SpouseDateOfBirth = table.Column<DateTime>(nullable: false),
                    SpouseName = table.Column<string>(nullable: true),
                    SpouseOtherName = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    SupportAlert = table.Column<bool>(nullable: false),
                    TargetId = table.Column<int>(nullable: false),
                    Telephone = table.Column<string>(nullable: true),
                    TinNumber = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: false),
                    TransactionAlert = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanServicings", x => x.LoanServicingCustId);
                    table.ForeignKey(
                        name: "FK_LoanServicings_CountryLists_CountryId",
                        column: x => x.CountryId,
                        principalTable: "CountryLists",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanServicings_CustomerTypes_CustomerTypeId",
                        column: x => x.CustomerTypeId,
                        principalTable: "CustomerTypes",
                        principalColumn: "CustomerTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanServicings_Ratings_RatingId",
                        column: x => x.RatingId,
                        principalTable: "Ratings",
                        principalColumn: "RatingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanServicings_Sectors_SectorId",
                        column: x => x.SectorId,
                        principalTable: "Sectors",
                        principalColumn: "SectorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanServicings_Targets_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Targets",
                        principalColumn: "TargetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanServicings_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyDirectorses",
                columns: table => new
                {
                    DirectorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Contacts = table.Column<string>(nullable: true),
                    CorporateCustId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    IdNumber = table.Column<string>(nullable: true),
                    IdType = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    OtherName = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyDirectorses", x => x.DirectorId);
                    table.ForeignKey(
                        name: "FK_CompanyDirectorses_Corporates_CorporateCustId",
                        column: x => x.CorporateCustId,
                        principalTable: "Corporates",
                        principalColumn: "CorporateCustId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyDirectorses_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanySignatories",
                columns: table => new
                {
                    SignatoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Contact = table.Column<string>(nullable: true),
                    CorporateCustId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    OtherName = table.Column<string>(nullable: true),
                    Picture = table.Column<string>(nullable: true),
                    Signature = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanySignatories", x => x.SignatoryId);
                    table.ForeignKey(
                        name: "FK_CompanySignatories_Corporates_CorporateCustId",
                        column: x => x.CorporateCustId,
                        principalTable: "Corporates",
                        principalColumn: "CorporateCustId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanySignatories_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RelatedDocuments",
                columns: table => new
                {
                    RelatedDocumentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CorporateCustId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatedDocuments", x => x.RelatedDocumentId);
                    table.ForeignKey(
                        name: "FK_RelatedDocuments_Corporates_CorporateCustId",
                        column: x => x.CorporateCustId,
                        principalTable: "Corporates",
                        principalColumn: "CorporateCustId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountTypes",
                columns: table => new
                {
                    AccountTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountAgeBeforeInterestIsPayable = table.Column<int>(nullable: true),
                    AccountType = table.Column<string>(nullable: true),
                    AccountTypeCode = table.Column<string>(nullable: true),
                    AllowFDBackDating = table.Column<string>(nullable: true),
                    AllowInterestOnDormantAccount = table.Column<bool>(nullable: false),
                    AllowOverdrawnBalance = table.Column<bool>(nullable: false),
                    ApprovalRuleId = table.Column<int>(nullable: false),
                    AutoApplyCOT = table.Column<bool>(nullable: false),
                    AutoApplySavingsInterest = table.Column<bool>(nullable: false),
                    AutoApplySavingsInterestFreq = table.Column<string>(nullable: true),
                    AutoCOTApplicationFreq = table.Column<string>(nullable: true),
                    BaseType = table.Column<string>(nullable: true),
                    COTTypeID = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    GLCodeForFDInterestAccrued = table.Column<int>(nullable: true),
                    GLCodeForFDInterestExpense = table.Column<int>(nullable: true),
                    GeneralLedgerCodeId = table.Column<int>(nullable: false),
                    GlCodeForFDPenalty = table.Column<int>(nullable: true),
                    InvestmentPlacement = table.Column<bool>(nullable: false),
                    LedgerId = table.Column<int>(nullable: false),
                    LedgerType = table.Column<string>(nullable: true),
                    MaximumFixedDepositTenor = table.Column<int>(nullable: true),
                    MinimumBalanceBeforeInterestIsPayable = table.Column<decimal>(type: "Money", nullable: false),
                    MinimumFixedDepositTenor = table.Column<int>(nullable: true),
                    MinimumInitialDepositAmount = table.Column<decimal>(type: "Money", nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    NumberOfDaysInYearForFDInterestCal = table.Column<int>(nullable: true),
                    NumberOfDaystoClassifyAccountAsDormant = table.Column<int>(nullable: false),
                    SavingsInterestCalculationMethod = table.Column<string>(nullable: true),
                    SavingsInterestTypeId = table.Column<int>(nullable: true),
                    SequenceFormatId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    WaitingPeriodBeforeFirstWithdrawal = table.Column<int>(nullable: false),
                    WithdrawalFrequency = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTypes", x => x.AccountTypeId);
                    table.ForeignKey(
                        name: "FK_AccountTypes_GeneralLedgerCodes_GeneralLedgerCodeId",
                        column: x => x.GeneralLedgerCodeId,
                        principalTable: "GeneralLedgerCodes",
                        principalColumn: "GeneralLedgerCodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountTypes_Ledgers_LedgerId",
                        column: x => x.LedgerId,
                        principalTable: "Ledgers",
                        principalColumn: "LedgerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountTypes_Banktiers_SavingsInterestTypeId",
                        column: x => x.SavingsInterestTypeId,
                        principalTable: "Banktiers",
                        principalColumn: "BanktiersId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountTypes_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JointCustomersKeys",
                columns: table => new
                {
                    JointCustKeysId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    IndividualCustId = table.Column<int>(nullable: false),
                    JointId = table.Column<int>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JointCustomersKeys", x => x.JointCustKeysId);
                    table.ForeignKey(
                        name: "FK_JointCustomersKeys_Individuals_IndividualCustId",
                        column: x => x.IndividualCustId,
                        principalTable: "Individuals",
                        principalColumn: "IndividualCustId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JointCustomersKeys_JointCustomers_JointId",
                        column: x => x.JointId,
                        principalTable: "JointCustomers",
                        principalColumn: "JointId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JointCustomersKeys_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountClosureDate = table.Column<DateTime>(nullable: true),
                    AccountNumber = table.Column<string>(nullable: true),
                    AccountStatus = table.Column<string>(nullable: true),
                    AccountTypeId = table.Column<int>(nullable: false),
                    ApprovedDateTime = table.Column<DateTime>(nullable: true),
                    ApprovedUserId = table.Column<int>(nullable: true),
                    Atm = table.Column<bool>(nullable: true),
                    AvailableBalance = table.Column<decimal>(type: "Money", nullable: false),
                    BranchId = table.Column<int>(nullable: false),
                    ClosedUserId = table.Column<int>(nullable: true),
                    ClosureReason = table.Column<string>(nullable: true),
                    CorporateCustId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    CustomerNumber = table.Column<string>(nullable: true),
                    GeneralLedgerCodeId = table.Column<int>(nullable: true),
                    IndividualCustId = table.Column<int>(nullable: true),
                    InterestOnSavingsId = table.Column<int>(nullable: true),
                    InternetBanking = table.Column<bool>(nullable: true),
                    JointId = table.Column<int>(nullable: true),
                    LoanPaymentAlert = table.Column<bool>(nullable: true),
                    MobileBanking = table.Column<bool>(nullable: true),
                    MobileMoney = table.Column<bool>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    OpenedDate = table.Column<DateTime>(nullable: true),
                    PurposeId = table.Column<string>(nullable: true),
                    RejectedUserId = table.Column<int>(nullable: true),
                    RelationsOfficerId = table.Column<int>(nullable: true),
                    SalaryAccount = table.Column<bool>(nullable: true),
                    SyncStatus = table.Column<bool>(nullable: false),
                    TotalBalance = table.Column<decimal>(type: "Money", nullable: false),
                    TransactionAlert = table.Column<bool>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_Accounts_AccountTypes_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalTable: "AccountTypes",
                        principalColumn: "AccountTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_BranchDetails_BranchId",
                        column: x => x.BranchId,
                        principalTable: "BranchDetails",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_Corporates_CorporateCustId",
                        column: x => x.CorporateCustId,
                        principalTable: "Corporates",
                        principalColumn: "CorporateCustId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_GeneralLedgerCodes_GeneralLedgerCodeId",
                        column: x => x.GeneralLedgerCodeId,
                        principalTable: "GeneralLedgerCodes",
                        principalColumn: "GeneralLedgerCodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_Individuals_IndividualCustId",
                        column: x => x.IndividualCustId,
                        principalTable: "Individuals",
                        principalColumn: "IndividualCustId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_JointCustomers_JointId",
                        column: x => x.JointId,
                        principalTable: "JointCustomers",
                        principalColumn: "JointId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountPopupMsg",
                columns: table => new
                {
                    AccountPopupMsgId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: false),
                    BranchId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountPopupMsg", x => x.AccountPopupMsgId);
                    table.ForeignKey(
                        name: "FK_AccountPopupMsg_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountPopupMsg_BranchDetails_BranchId",
                        column: x => x.BranchId,
                        principalTable: "BranchDetails",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cheques",
                columns: table => new
                {
                    ChequeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: false),
                    AutoClear = table.Column<bool>(nullable: true),
                    BranchId = table.Column<int>(nullable: false),
                    ChequeAmount = table.Column<decimal>(type: "Money", nullable: false),
                    ChequeNumber = table.Column<string>(nullable: true),
                    ClearedDate = table.Column<DateTime>(nullable: true),
                    ClearedUserId = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    Note = table.Column<string>(nullable: true),
                    ReturnedDate = table.Column<DateTime>(nullable: true),
                    ReturnedUserId = table.Column<int>(nullable: true),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cheques", x => x.ChequeId);
                    table.ForeignKey(
                        name: "FK_Cheques_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cheques_BranchDetails_BranchId",
                        column: x => x.BranchId,
                        principalTable: "BranchDetails",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FixedDeposit",
                columns: table => new
                {
                    FixedDepositId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: false),
                    AutoApplyFixedDepositInterest = table.Column<int>(nullable: true),
                    DaysToAutoApplyFixedDepositInterest = table.Column<int>(nullable: true),
                    FixedDepositDailyInterest = table.Column<decimal>(type: "Money", nullable: true),
                    FixedDepositFundingSourceAccountId = table.Column<int>(nullable: false),
                    FixedDepositInterestAccrued = table.Column<decimal>(type: "Money", nullable: true),
                    FixedDepositInterestAmount = table.Column<decimal>(type: "Money", nullable: true),
                    FixedDepositInterestAutoApplyEndDate = table.Column<DateTime>(nullable: true),
                    FixedDepositInterestRate = table.Column<decimal>(nullable: true),
                    FixedDepositMaturityAmount = table.Column<decimal>(type: "Money", nullable: true),
                    FixedDepositMaturityDate = table.Column<DateTime>(nullable: true),
                    FixedDepositPeriod = table.Column<int>(nullable: true),
                    FixedDepositPrincipal = table.Column<decimal>(type: "Money", nullable: true),
                    FixedDepositServicingAccountId = table.Column<int>(nullable: true),
                    InvIntLastAccruedDate = table.Column<DateTime>(nullable: true),
                    InvestmentAlert = table.Column<bool>(nullable: true),
                    NewInterestRate = table.Column<decimal>(nullable: true),
                    NewPeriod = table.Column<int>(nullable: true),
                    NextFixedDepositInterestAutoApplyDate = table.Column<DateTime>(nullable: true),
                    RolloverInterest = table.Column<bool>(nullable: true),
                    RolloverPrincipal = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedDeposit", x => x.FixedDepositId);
                    table.ForeignKey(
                        name: "FK_FixedDeposit_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IssuedChequeBooks",
                columns: table => new
                {
                    IssuedChequeBookId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: false),
                    BranchId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    EndNumber = table.Column<int>(nullable: false),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    StartNumber = table.Column<int>(nullable: false),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssuedChequeBooks", x => x.IssuedChequeBookId);
                    table.ForeignKey(
                        name: "FK_IssuedChequeBooks_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssuedChequeBooks_BranchDetails_BranchId",
                        column: x => x.BranchId,
                        principalTable: "BranchDetails",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Liens",
                columns: table => new
                {
                    LienId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(type: "Money", nullable: false),
                    ApprovedDate = table.Column<DateTime>(nullable: true),
                    ApprovedNote = table.Column<string>(nullable: true),
                    ApprovedUserId = table.Column<int>(nullable: true),
                    AutoRelease = table.Column<bool>(nullable: true),
                    BranchId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    LienNumber = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    Reference = table.Column<string>(nullable: true),
                    RejectedNote = table.Column<string>(nullable: true),
                    RejectedUserId = table.Column<int>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Liens", x => x.LienId);
                    table.ForeignKey(
                        name: "FK_Liens_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Liens_BranchDetails_BranchId",
                        column: x => x.BranchId,
                        principalTable: "BranchDetails",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Overdrafts",
                columns: table => new
                {
                    OverdraftId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(type: "Money", nullable: true),
                    ApprovedDate = table.Column<DateTime>(nullable: true),
                    ApprovedNote = table.Column<string>(nullable: true),
                    ApprovedUserId = table.Column<int>(nullable: true),
                    AutoRelease = table.Column<bool>(nullable: true),
                    BranchId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    InterestAccrued = table.Column<decimal>(type: "Money", nullable: true),
                    InterestRate = table.Column<decimal>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    OverdraftNumber = table.Column<string>(nullable: true),
                    Reference = table.Column<string>(nullable: true),
                    RejectedDate = table.Column<DateTime>(nullable: true),
                    RejectedNote = table.Column<string>(nullable: true),
                    RejectedUserId = table.Column<int>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Overdrafts", x => x.OverdraftId);
                    table.ForeignKey(
                        name: "FK_Overdrafts_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Overdrafts_BranchDetails_BranchId",
                        column: x => x.BranchId,
                        principalTable: "BranchDetails",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SmsLog",
                columns: table => new
                {
                    SmsLogId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: false),
                    BranchId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    ResponseCode = table.Column<int>(nullable: true),
                    SentDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<bool>(nullable: true),
                    TransCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsLog", x => x.SmsLogId);
                    table.ForeignKey(
                        name: "FK_SmsLog_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SmsLog_BranchDetails_BranchId",
                        column: x => x.BranchId,
                        principalTable: "BranchDetails",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: true),
                    Balance = table.Column<decimal>(type: "Money", nullable: true),
                    BranchId = table.Column<int>(nullable: false),
                    ChequeNumber = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    Credit = table.Column<decimal>(type: "Money", nullable: false),
                    Debit = table.Column<decimal>(type: "Money", nullable: false),
                    GeneralLedgerCodeId = table.Column<int>(nullable: true),
                    LedgerId = table.Column<int>(nullable: false),
                    LedgerType = table.Column<string>(nullable: true),
                    LoanId = table.Column<int>(nullable: true),
                    MacAddress = table.Column<string>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    ReconciledSessionDate = table.Column<DateTime>(nullable: true),
                    ReconciledState = table.Column<bool>(nullable: true),
                    ReconciledUserId = table.Column<int>(nullable: true),
                    Reference = table.Column<string>(nullable: true),
                    SessionDate = table.Column<DateTime>(nullable: true),
                    SmsStatus = table.Column<bool>(nullable: true),
                    TransCode = table.Column<string>(nullable: true),
                    TransSource = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_BranchDetails_BranchId",
                        column: x => x.BranchId,
                        principalTable: "BranchDetails",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_GeneralLedgerCodes_GeneralLedgerCodeId",
                        column: x => x.GeneralLedgerCodeId,
                        principalTable: "GeneralLedgerCodes",
                        principalColumn: "GeneralLedgerCodeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Ledgers_LedgerId",
                        column: x => x.LedgerId,
                        principalTable: "Ledgers",
                        principalColumn: "LedgerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "LoanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransCodeItems",
                columns: table => new
                {
                    TransCodeItemsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<int>(nullable: true),
                    BranchId = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: true),
                    CreatedUserId = table.Column<int>(nullable: true),
                    Credit = table.Column<decimal>(type: "Money", nullable: true),
                    Debit = table.Column<decimal>(type: "Money", nullable: true),
                    GeneralLedgerCodeId = table.Column<int>(nullable: true),
                    LedgerType = table.Column<string>(maxLength: 10, nullable: true),
                    LoanId = table.Column<int>(nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    ModifiedUserId = table.Column<int>(nullable: true),
                    Reference = table.Column<string>(maxLength: 100, nullable: true),
                    SessionDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    TransCode = table.Column<string>(maxLength: 50, nullable: true),
                    TransSource = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransCodeItems", x => x.TransCodeItemsId);
                    table.ForeignKey(
                        name: "FK_TransCodeItems_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransCodeItems_BranchDetails_BranchId",
                        column: x => x.BranchId,
                        principalTable: "BranchDetails",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransCodeItems_GeneralLedgerCodes_GeneralLedgerCodeId",
                        column: x => x.GeneralLedgerCodeId,
                        principalTable: "GeneralLedgerCodes",
                        principalColumn: "GeneralLedgerCodeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountPopupMsg_AccountId",
                table: "AccountPopupMsg",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountPopupMsg_BranchId",
                table: "AccountPopupMsg",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountTypeId",
                table: "Accounts",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_BranchId",
                table: "Accounts",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CorporateCustId",
                table: "Accounts",
                column: "CorporateCustId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_GeneralLedgerCodeId",
                table: "Accounts",
                column: "GeneralLedgerCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_IndividualCustId",
                table: "Accounts",
                column: "IndividualCustId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_JointId",
                table: "Accounts",
                column: "JointId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserId",
                table: "Accounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypes_GeneralLedgerCodeId",
                table: "AccountTypes",
                column: "GeneralLedgerCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypes_LedgerId",
                table: "AccountTypes",
                column: "LedgerId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypes_SavingsInterestTypeId",
                table: "AccountTypes",
                column: "SavingsInterestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypes_UserId",
                table: "AccountTypes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRules_BranchId",
                table: "ApprovalRules",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserId",
                table: "AspNetUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BankDetails_UserId",
                table: "BankDetails",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Banktiers_GeneralLedgerCodeId",
                table: "Banktiers",
                column: "GeneralLedgerCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchDetails_UserId",
                table: "BranchDetails",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cheques_AccountId",
                table: "Cheques",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Cheques_BranchId",
                table: "Cheques",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDirectorses_CorporateCustId",
                table: "CompanyDirectorses",
                column: "CorporateCustId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDirectorses_UserId",
                table: "CompanyDirectorses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanySignatories_CorporateCustId",
                table: "CompanySignatories",
                column: "CorporateCustId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanySignatories_UserId",
                table: "CompanySignatories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Corporates_CountryListCountryId",
                table: "Corporates",
                column: "CountryListCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Corporates_UserId",
                table: "Corporates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CountryLists_UserId",
                table: "CountryLists",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedDeposit_AccountId",
                table: "FixedDeposit",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLedgerCodes_MainGeneralLedgerCodeId",
                table: "GeneralLedgerCodes",
                column: "MainGeneralLedgerCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralLedgerCodes_UserId",
                table: "GeneralLedgerCodes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Individuals_CountryId",
                table: "Individuals",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Individuals_RatingId",
                table: "Individuals",
                column: "RatingId");

            migrationBuilder.CreateIndex(
                name: "IX_Individuals_SectorId",
                table: "Individuals",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Individuals_TargetId",
                table: "Individuals",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_Individuals_UserId",
                table: "Individuals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuedChequeBooks_AccountId",
                table: "IssuedChequeBooks",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuedChequeBooks_BranchId",
                table: "IssuedChequeBooks",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_JointCustomers_UserId",
                table: "JointCustomers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_JointCustomersKeys_IndividualCustId",
                table: "JointCustomersKeys",
                column: "IndividualCustId");

            migrationBuilder.CreateIndex(
                name: "IX_JointCustomersKeys_JointId",
                table: "JointCustomersKeys",
                column: "JointId");

            migrationBuilder.CreateIndex(
                name: "IX_JointCustomersKeys_UserId",
                table: "JointCustomersKeys",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Ledgers_UserId",
                table: "Ledgers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Liens_AccountId",
                table: "Liens",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Liens_BranchId",
                table: "Liens",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanServicings_CountryId",
                table: "LoanServicings",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanServicings_CustomerTypeId",
                table: "LoanServicings",
                column: "CustomerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanServicings_RatingId",
                table: "LoanServicings",
                column: "RatingId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanServicings_SectorId",
                table: "LoanServicings",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanServicings_TargetId",
                table: "LoanServicings",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanServicings_UserId",
                table: "LoanServicings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Overdrafts_AccountId",
                table: "Overdrafts",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Overdrafts_BranchId",
                table: "Overdrafts",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_UserId",
                table: "Ratings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RelatedDocuments_CorporateCustId",
                table: "RelatedDocuments",
                column: "CorporateCustId");

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_UserId",
                table: "Sectors",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceConfig_BranchId",
                table: "ServiceConfig",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionManager_BranchId",
                table: "SessionManager",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsLog_AccountId",
                table: "SmsLog",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsLog_BranchId",
                table: "SmsLog",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Sweeps_BranchId",
                table: "Sweeps",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Targets_UserId",
                table: "Targets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Teller_BranchId",
                table: "Teller",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Teller_GeneralLedgerCodeId",
                table: "Teller",
                column: "GeneralLedgerCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Teller_UserId",
                table: "Teller",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId",
                table: "Transactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BranchId",
                table: "Transactions",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_GeneralLedgerCodeId",
                table: "Transactions",
                column: "GeneralLedgerCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_LedgerId",
                table: "Transactions",
                column: "LedgerId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_LoanId",
                table: "Transactions",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_TransCodeItems_AccountId",
                table: "TransCodeItems",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TransCodeItems_BranchId",
                table: "TransCodeItems",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_TransCodeItems_GeneralLedgerCodeId",
                table: "TransCodeItems",
                column: "GeneralLedgerCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_GroupId",
                table: "UserGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_UserId",
                table: "UserGroups",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountPopupMsg");

            migrationBuilder.DropTable(
                name: "ApprovalRules");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BankDetails");

            migrationBuilder.DropTable(
                name: "Cheques");

            migrationBuilder.DropTable(
                name: "CommonSequences");

            migrationBuilder.DropTable(
                name: "CompanyDirectorses");

            migrationBuilder.DropTable(
                name: "CompanySignatories");

            migrationBuilder.DropTable(
                name: "FileUploads");

            migrationBuilder.DropTable(
                name: "FixedDeposit");

            migrationBuilder.DropTable(
                name: "IssuedChequeBooks");

            migrationBuilder.DropTable(
                name: "JointCustomersKeys");

            migrationBuilder.DropTable(
                name: "Liens");

            migrationBuilder.DropTable(
                name: "LoanServicings");

            migrationBuilder.DropTable(
                name: "Overdrafts");

            migrationBuilder.DropTable(
                name: "RelatedDocuments");

            migrationBuilder.DropTable(
                name: "ServiceConfig");

            migrationBuilder.DropTable(
                name: "SessionManager");

            migrationBuilder.DropTable(
                name: "SmsConfig");

            migrationBuilder.DropTable(
                name: "SmsLog");

            migrationBuilder.DropTable(
                name: "Sweeps");

            migrationBuilder.DropTable(
                name: "Teller");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "TransCodeItems");

            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "CustomerTypes");

            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "AccountTypes");

            migrationBuilder.DropTable(
                name: "BranchDetails");

            migrationBuilder.DropTable(
                name: "Corporates");

            migrationBuilder.DropTable(
                name: "Individuals");

            migrationBuilder.DropTable(
                name: "JointCustomers");

            migrationBuilder.DropTable(
                name: "Ledgers");

            migrationBuilder.DropTable(
                name: "Banktiers");

            migrationBuilder.DropTable(
                name: "CountryLists");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "Sectors");

            migrationBuilder.DropTable(
                name: "Targets");

            migrationBuilder.DropTable(
                name: "GeneralLedgerCodes");

            migrationBuilder.DropTable(
                name: "MainGeneralLedgerCodes");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
