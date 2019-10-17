using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class InitTerminate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TerminateCustomerClassChanges",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    CustomerID = table.Column<int>(nullable: true),
                    From = table.Column<string>(nullable: true),
                    FromID = table.Column<int>(nullable: true),
                    To = table.Column<string>(nullable: true),
                    ToID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminateCustomerClassChanges", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TerminateCustomerClassChanges_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TerminateCustomerPoints",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChannelType = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    CustomerChanal = table.Column<int>(nullable: false),
                    CustomerClassName = table.Column<string>(nullable: true),
                    CustomerID = table.Column<int>(nullable: false),
                    EffectiveDate = table.Column<DateTime>(nullable: true),
                    ExpiryDate = table.Column<DateTime>(nullable: true),
                    InsuranceClass = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OrderNo = table.Column<string>(nullable: true),
                    OutletCode = table.Column<string>(nullable: true),
                    Package = table.Column<string>(nullable: true),
                    Point = table.Column<int>(nullable: false),
                    PolicyNo = table.Column<string>(nullable: true),
                    PreviousPolicyNo = table.Column<string>(nullable: true),
                    ProductID = table.Column<int>(nullable: true),
                    ProjectCode = table.Column<string>(nullable: true),
                    ProjectName = table.Column<string>(nullable: true),
                    PurchaseAmt = table.Column<decimal>(nullable: false),
                    Source = table.Column<string>(nullable: true),
                    Subclass = table.Column<string>(nullable: true),
                    TransacionTypeID = table.Column<int>(nullable: false),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminateCustomerPoints", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TerminateCustomerPoints_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TerminateCustomers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BCryptPwd = table.Column<string>(nullable: true),
                    CUR_Address = table.Column<string>(nullable: true),
                    CUR_AddressEn = table.Column<string>(nullable: true),
                    CUR_Aumper = table.Column<int>(nullable: true),
                    CUR_AumperEn = table.Column<int>(nullable: true),
                    CUR_Building = table.Column<string>(nullable: true),
                    CUR_BuildingEn = table.Column<string>(nullable: true),
                    CUR_HouseNo = table.Column<string>(nullable: true),
                    CUR_HouseNoEn = table.Column<string>(nullable: true),
                    CUR_Lane = table.Column<string>(nullable: true),
                    CUR_LaneEn = table.Column<string>(nullable: true),
                    CUR_Moo = table.Column<string>(nullable: true),
                    CUR_MooEn = table.Column<string>(nullable: true),
                    CUR_Province = table.Column<int>(nullable: true),
                    CUR_ProvinceEn = table.Column<int>(nullable: true),
                    CUR_Road = table.Column<string>(nullable: true),
                    CUR_RoadEn = table.Column<string>(nullable: true),
                    CUR_Soi = table.Column<string>(nullable: true),
                    CUR_SoiEn = table.Column<string>(nullable: true),
                    CUR_Tumbon = table.Column<int>(nullable: true),
                    CUR_TumbonEn = table.Column<int>(nullable: true),
                    CUR_VillageName = table.Column<string>(nullable: true),
                    CUR_VillageNameEn = table.Column<string>(nullable: true),
                    CUR_VillageNo = table.Column<string>(nullable: true),
                    CUR_VillageNoEn = table.Column<string>(nullable: true),
                    CUR_ZipCode = table.Column<string>(nullable: true),
                    CUR_ZipCodeEn = table.Column<string>(nullable: true),
                    Channel = table.Column<int>(nullable: false),
                    ChannelUpdate = table.Column<int>(nullable: false),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    CustomerClassID = table.Column<int>(nullable: true),
                    CustomerID = table.Column<int>(nullable: false),
                    CustomerNo = table.Column<string>(nullable: true),
                    DOB = table.Column<DateTime>(nullable: true),
                    DoSendReisterEmail = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    FacebookFlag = table.Column<string>(nullable: true),
                    FacebookID = table.Column<string>(nullable: true),
                    FirstLogedIn = table.Column<bool>(nullable: false),
                    FriendCode = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    IDCard = table.Column<string>(nullable: true),
                    IIAIgnoreSyned = table.Column<bool>(nullable: false),
                    IIASyned = table.Column<bool>(nullable: false),
                    IsDhiMember = table.Column<bool>(nullable: false),
                    LineID = table.Column<string>(nullable: true),
                    MoblieNo = table.Column<string>(nullable: true),
                    NameEn = table.Column<string>(nullable: true),
                    NameTh = table.Column<string>(nullable: false),
                    Passport = table.Column<string>(nullable: true),
                    PrefixEn = table.Column<int>(nullable: true),
                    PrefixTh = table.Column<int>(nullable: true),
                    PromotionCode = table.Column<string>(nullable: true),
                    RefCode = table.Column<string>(nullable: true),
                    RegGeneratedPoint = table.Column<bool>(nullable: false),
                    ResetedPwd = table.Column<bool>(nullable: false),
                    SentReisterEmail = table.Column<bool>(nullable: false),
                    SentReisterMsg = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Success = table.Column<bool>(nullable: true),
                    SurNameEn = table.Column<string>(nullable: true),
                    SurNameTh = table.Column<string>(nullable: false),
                    Syned = table.Column<bool>(nullable: false),
                    TelNo = table.Column<string>(nullable: true),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true),
                    UpdatedAllRequired = table.Column<bool>(nullable: false),
                    UserID = table.Column<int>(nullable: true),
                    UserLevel = table.Column<int>(nullable: false),
                    WorkTelNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminateCustomers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TerminateCustomers_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TerminateMobilePoints",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Channel = table.Column<string>(nullable: false),
                    Create_On = table.Column<DateTime>(nullable: true),
                    CustomerID = table.Column<int>(nullable: false),
                    IDCard = table.Column<string>(nullable: true),
                    OrderNo = table.Column<string>(nullable: true),
                    Package = table.Column<string>(nullable: false),
                    Passport = table.Column<string>(nullable: true),
                    Point = table.Column<decimal>(nullable: false),
                    PolicyNo = table.Column<string>(nullable: true),
                    Product = table.Column<string>(nullable: false),
                    PurchaseAmt = table.Column<decimal>(nullable: false),
                    Source = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminateMobilePoints", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TerminatePointAdjusts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConditionCode = table.Column<string>(nullable: true),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    CustomerChanal = table.Column<int>(nullable: false),
                    CustomerID = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Point = table.Column<int>(nullable: false),
                    PurchaseAmt = table.Column<decimal>(nullable: false),
                    TransacionTypeID = table.Column<int>(nullable: false),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminatePointAdjusts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TerminatePointAdjusts_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TerminateRedeems",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    Confirmed = table.Column<bool>(nullable: false),
                    CustomerClassName = table.Column<string>(nullable: true),
                    CustomerID = table.Column<int>(nullable: false),
                    MerchantName = table.Column<string>(nullable: true),
                    Point = table.Column<decimal>(nullable: false),
                    PrivilegeCodeType = table.Column<int>(nullable: false),
                    PrivilegeID = table.Column<int>(nullable: false),
                    PrivilegeName = table.Column<string>(nullable: true),
                    RedeemCode = table.Column<string>(nullable: true),
                    RedeemDate = table.Column<DateTime>(nullable: true),
                    RedeemType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminateRedeems", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TerminateUsers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    CustomerID = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true),
                    UserName = table.Column<string>(nullable: false),
                    UserRoleID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminateUsers", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TerminateCustomerClassChanges_CustomerID",
                table: "TerminateCustomerClassChanges",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_TerminateCustomerPoints_CustomerID",
                table: "TerminateCustomerPoints",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_TerminateCustomers_UserID",
                table: "TerminateCustomers",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_TerminatePointAdjusts_CustomerID",
                table: "TerminatePointAdjusts",
                column: "CustomerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TerminateCustomerClassChanges");

            migrationBuilder.DropTable(
                name: "TerminateCustomerPoints");

            migrationBuilder.DropTable(
                name: "TerminateCustomers");

            migrationBuilder.DropTable(
                name: "TerminateMobilePoints");

            migrationBuilder.DropTable(
                name: "TerminatePointAdjusts");

            migrationBuilder.DropTable(
                name: "TerminateRedeems");

            migrationBuilder.DropTable(
                name: "TerminateUsers");
        }
    }
}
