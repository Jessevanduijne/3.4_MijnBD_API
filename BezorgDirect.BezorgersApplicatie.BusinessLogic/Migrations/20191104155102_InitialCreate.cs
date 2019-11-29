using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DelivererId = table.Column<Guid>(nullable: false),
                    DeliveryId = table.Column<Guid>(nullable: false),
                    Category = table.Column<int>(nullable: false),
                    Rating = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DelivererId = table.Column<Guid>(nullable: false),
                    DeliveryId = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    AcceptedAt = table.Column<DateTime>(nullable: true),
                    RefusedAt = table.Column<DateTime>(nullable: true),
                    ExpiredAt = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DelivererId = table.Column<Guid>(nullable: true),
                    CustomerPhoneNumber = table.Column<string>(maxLength: 10, nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    Vehicle = table.Column<int>(nullable: false),
                    StartedAtId = table.Column<Guid>(nullable: true),
                    WarehouseDistanceInKilometers = table.Column<double>(nullable: true),
                    WarehouseETA = table.Column<DateTime>(nullable: true),
                    WarehouseId = table.Column<Guid>(nullable: false),
                    WarehousePickUpAt = table.Column<DateTime>(nullable: true),
                    CustomerDistanceInKilometers = table.Column<double>(nullable: true),
                    CustomerETA = table.Column<DateTime>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: false),
                    CurrentId = table.Column<Guid>(nullable: true),
                    DeliveredAt = table.Column<DateTime>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    Tip = table.Column<double>(nullable: true),
                    PaymentMethod = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EmailAddress = table.Column<string>(nullable: false),
                    Hash = table.Column<string>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 10, nullable: true),
                    HomeId = table.Column<Guid>(nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: true),
                    Range = table.Column<int>(nullable: true),
                    Vehicle = table.Column<int>(nullable: true),
                    Fare = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Availabilities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DelivererId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    StartTime = table.Column<TimeSpan>(nullable: false),
                    EndTime = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Availabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Availabilities_Users_DelivererId",
                        column: x => x.DelivererId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(maxLength: 6, nullable: true),
                    Place = table.Column<string>(nullable: true),
                    IsWarehouse = table.Column<bool>(nullable: false),
                    DelivererId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Users_DelivererId",
                        column: x => x.DelivererId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            /*migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Address", "DelivererId", "IsWarehouse", "Latitude", "Longitude", "Place", "PostalCode" },
                values: new object[] { new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), "Lage Duin 38-14", null, false, 52.322403999999999, 4.5969259999999998, "Bennebroek", "2121CH" });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Address", "DelivererId", "IsWarehouse", "Latitude", "Longitude", "Place", "PostalCode" },
                values: new object[] { new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), "Bijdorplaan 15", null, false, 52.389454000000001, 4.6128770000000001, "Haarlem", "2015CE" });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Address", "DelivererId", "IsWarehouse", "Latitude", "Longitude", "Place", "PostalCode" },
                values: new object[] { new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), "Vlietweg 16", null, true, 52.425825000000003, 4.6467109999999998, "Santpoort-Noord", "2071KW" });

            migrationBuilder.InsertData(
                table: "Deliveries",
                columns: new[] { "Id", "CurrentId", "CustomerDistanceInKilometers", "CustomerETA", "CustomerId", "CustomerPhoneNumber", "DeliveredAt", "DelivererId", "DueDate", "PaymentMethod", "Price", "StartedAtId", "Status", "Tip", "Vehicle", "WarehouseDistanceInKilometers", "WarehouseETA", "WarehouseId", "WarehousePickUpAt" },
                values: new object[] { new Guid("082f1eda-2985-4d2b-8add-80487f8edf66"), null, 5.7000000000000002, new DateTime(2019, 11, 4, 17, 0, 2, 148, DateTimeKind.Local).AddTicks(8469), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), "0612345678", null, null, new DateTime(2019, 11, 4, 18, 51, 2, 148, DateTimeKind.Local).AddTicks(8447), 0, 53.68, null, 1, null, 4, null, null, new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Discriminator", "EmailAddress", "Hash", "Token", "DateOfBirth", "Fare", "HomeId", "PhoneNumber", "Range", "Vehicle" },
                values: new object[,]
                {
                    { new Guid("1c8d26ac-0879-4bd4-aa45-ede7206c315d"), "Deliverer", "test1@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, new DateTime(1997, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 5.4100000000000001, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), "0612345678", 5, 4 },
                    { new Guid("a6578db0-5e35-4a6b-967b-2d546e16b6f7"), "Deliverer", "test2@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, new DateTime(1997, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 5.4100000000000001, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), "0612345678", 40, 4 },
                    { new Guid("03ab5306-01a8-400d-a43a-2edef178a187"), "Deliverer", "test3@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, new DateTime(1997, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 5.4100000000000001, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), "0612345678", 40, 4 },
                    { new Guid("4a18c314-3c45-4eea-b893-680eb09c1abe"), "Deliverer", "test4@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, new DateTime(1997, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 5.4100000000000001, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), "0612345678", 40, 4 },
                    { new Guid("b97b134c-9f3d-4bef-9085-85b16b2f0cff"), "Deliverer", "test5@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, new DateTime(1997, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 5.4100000000000001, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), "0612345678", 40, 4 },
                    { new Guid("1566979b-9fa6-4a51-8e67-08509ce79106"), "Deliverer", "test6@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, new DateTime(1997, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 5.4100000000000001, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), "0612345678", 40, 2 },
                    { new Guid("77f3c372-e321-4263-ba7a-0f86ce51de56"), "Deliverer", "test7@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, new DateTime(1997, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 5.4100000000000001, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), "0612345678", 40, 4 },
                    { new Guid("fa8ec550-2863-4f01-82df-488d66004a60"), "Deliverer", "test8@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, new DateTime(1997, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 5.4100000000000001, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), "0612345678", 40, 4 },
                    { new Guid("c26682bd-d8bf-4698-a00e-02d059a2498d"), "Deliverer", "test9@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, new DateTime(1997, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 5.4100000000000001, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), "0612345678", 40, 4 },
                    { new Guid("546210b1-6ec8-4d64-b058-b3fa7e0740aa"), "Deliverer", "test10@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, new DateTime(1997, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 5.4100000000000001, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), "0612345678", 40, 4 },
                    { new Guid("5fc08418-6e23-4de8-88b0-1c513ddaf40d"), "Deliverer", "test11@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, new DateTime(1997, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 5.4100000000000001, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), "0612345678", 40, 4 },
                    { new Guid("f2f458f3-819e-46b3-be44-554f6a2fad93"), "Deliverer", "test12@test.test", "bc547750b92797f955b36112cc9bdd5cddf7d0862151d03a167ada8995aa24a9ad24610b36a68bc02da24141ee51670aea13ed6469099a4453f335cb239db5da", null, new DateTime(1997, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 5.4100000000000001, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), "0612345678", 40, 4 }
                });

            migrationBuilder.InsertData(
                table: "Availabilities",
                columns: new[] { "Id", "Date", "DelivererId", "EndTime", "StartTime" },
                values: new object[,]
                {
                    { new Guid("e210c6e9-a284-4733-89ec-47bd8da81f57"), new DateTime(2019, 11, 4, 0, 0, 0, 0, DateTimeKind.Local), new Guid("1c8d26ac-0879-4bd4-aa45-ede7206c315d"), new TimeSpan(699621446676), new TimeSpan(606621446670) },
                    { new Guid("bbb93360-88f1-4e87-8406-7fc5bc587652"), new DateTime(2019, 11, 4, 0, 0, 0, 0, DateTimeKind.Local), new Guid("546210b1-6ec8-4d64-b058-b3fa7e0740aa"), new TimeSpan(699621456823), new TimeSpan(606621456818) },
                    { new Guid("64a84684-529d-4bf4-9213-39c0c64226e8"), new DateTime(2019, 11, 4, 0, 0, 0, 0, DateTimeKind.Local), new Guid("fa8ec550-2863-4f01-82df-488d66004a60"), new TimeSpan(699621456777), new TimeSpan(606621456774) },
                    { new Guid("3f414f30-48c3-4108-bf1e-88ec9007f45e"), new DateTime(2019, 11, 4, 0, 0, 0, 0, DateTimeKind.Local), new Guid("77f3c372-e321-4263-ba7a-0f86ce51de56"), new TimeSpan(699621456760), new TimeSpan(606621456754) },
                    { new Guid("e13a9f39-0d81-4bbf-af63-29615e981171"), new DateTime(2019, 11, 4, 0, 0, 0, 0, DateTimeKind.Local), new Guid("5fc08418-6e23-4de8-88b0-1c513ddaf40d"), new TimeSpan(699621456842), new TimeSpan(606621456836) },
                    { new Guid("f56dd88e-f985-4daa-afee-ff377f6008a7"), new DateTime(2019, 11, 4, 0, 0, 0, 0, DateTimeKind.Local), new Guid("b97b134c-9f3d-4bef-9085-85b16b2f0cff"), new TimeSpan(699621456726), new TimeSpan(606621456721) },
                    { new Guid("4bdead21-875e-46b3-9dcf-4a02ae8cede4"), new DateTime(2019, 11, 4, 0, 0, 0, 0, DateTimeKind.Local), new Guid("1566979b-9fa6-4a51-8e67-08509ce79106"), new TimeSpan(699621456742), new TimeSpan(606621456737) },
                    { new Guid("32325d52-a8c8-47b1-9b81-8d60b86e4120"), new DateTime(2019, 11, 4, 0, 0, 0, 0, DateTimeKind.Local), new Guid("f2f458f3-819e-46b3-be44-554f6a2fad93"), new TimeSpan(699621456859), new TimeSpan(606621456855) },
                    { new Guid("6e1acab9-a426-43f1-81e9-e9a98b88398a"), new DateTime(2019, 11, 4, 0, 0, 0, 0, DateTimeKind.Local), new Guid("a6578db0-5e35-4a6b-967b-2d546e16b6f7"), new TimeSpan(699621456335), new TimeSpan(606621456329) },
                    { new Guid("8216c5a9-bda8-4294-82ec-2af25bd0b19e"), new DateTime(2019, 11, 4, 0, 0, 0, 0, DateTimeKind.Local), new Guid("4a18c314-3c45-4eea-b893-680eb09c1abe"), new TimeSpan(699621456692), new TimeSpan(606621456686) },
                    { new Guid("41d5e2c0-ed75-44a7-987a-b6773a18eaef"), new DateTime(2019, 11, 4, 0, 0, 0, 0, DateTimeKind.Local), new Guid("c26682bd-d8bf-4698-a00e-02d059a2498d"), new TimeSpan(699621456797), new TimeSpan(606621456791) }
                });

            migrationBuilder.InsertData(
                table: "Deliveries",
                columns: new[] { "Id", "CurrentId", "CustomerDistanceInKilometers", "CustomerETA", "CustomerId", "CustomerPhoneNumber", "DeliveredAt", "DelivererId", "DueDate", "PaymentMethod", "Price", "StartedAtId", "Status", "Tip", "Vehicle", "WarehouseDistanceInKilometers", "WarehouseETA", "WarehouseId", "WarehousePickUpAt" },
                values: new object[,]
                {
                    { new Guid("c4f626f8-e626-45b9-8548-b1d922a49e0d"), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), 5.7000000000000002, new DateTime(2019, 11, 4, 16, 51, 2, 149, DateTimeKind.Local).AddTicks(1848), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), "0612345678", new DateTime(2019, 11, 4, 17, 33, 2, 149, DateTimeKind.Local).AddTicks(1854), new Guid("5fc08418-6e23-4de8-88b0-1c513ddaf40d"), new DateTime(2019, 11, 4, 18, 51, 2, 149, DateTimeKind.Local).AddTicks(1830), 1, 14.26, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), 4, null, 4, 13.6, new DateTime(2019, 11, 4, 16, 21, 2, 149, DateTimeKind.Local).AddTicks(1838), new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), new DateTime(2019, 11, 4, 16, 42, 2, 149, DateTimeKind.Local).AddTicks(1843) },
                    { new Guid("0415d421-3359-4117-a697-092186ae2569"), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), 5.7000000000000002, new DateTime(2019, 11, 4, 16, 51, 2, 149, DateTimeKind.Local).AddTicks(1008), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), "0612345678", new DateTime(2019, 11, 4, 17, 37, 2, 149, DateTimeKind.Local).AddTicks(1014), new Guid("c26682bd-d8bf-4698-a00e-02d059a2498d"), new DateTime(2019, 11, 4, 18, 51, 2, 149, DateTimeKind.Local).AddTicks(989), 1, 14.26, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), 4, null, 4, 13.6, new DateTime(2019, 11, 4, 16, 21, 2, 149, DateTimeKind.Local).AddTicks(997), new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), new DateTime(2019, 11, 4, 16, 42, 2, 149, DateTimeKind.Local).AddTicks(1003) },
                    { new Guid("4aea94b7-a21f-472f-be50-154fead4ac8c"), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), 5.7000000000000002, new DateTime(2019, 11, 4, 16, 51, 2, 149, DateTimeKind.Local).AddTicks(2106), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), "0612345678", new DateTime(2019, 11, 4, 17, 24, 2, 149, DateTimeKind.Local).AddTicks(2109), new Guid("5fc08418-6e23-4de8-88b0-1c513ddaf40d"), new DateTime(2019, 11, 4, 18, 51, 2, 149, DateTimeKind.Local).AddTicks(2089), 1, 14.26, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), 4, null, 4, 13.6, new DateTime(2019, 11, 4, 16, 21, 2, 149, DateTimeKind.Local).AddTicks(2096), new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), new DateTime(2019, 11, 4, 16, 42, 2, 149, DateTimeKind.Local).AddTicks(2102) },
                    { new Guid("8800377f-f8c9-4db8-8fed-60b4fa2b4bb6"), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), 5.7000000000000002, new DateTime(2019, 11, 4, 16, 51, 2, 149, DateTimeKind.Local).AddTicks(1513), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), "0612345678", new DateTime(2019, 11, 4, 17, 19, 2, 149, DateTimeKind.Local).AddTicks(1516), new Guid("546210b1-6ec8-4d64-b058-b3fa7e0740aa"), new DateTime(2019, 11, 4, 18, 51, 2, 149, DateTimeKind.Local).AddTicks(1497), 1, 14.26, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), 4, null, 4, 13.6, new DateTime(2019, 11, 4, 16, 21, 2, 149, DateTimeKind.Local).AddTicks(1504), new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), new DateTime(2019, 11, 4, 16, 42, 2, 149, DateTimeKind.Local).AddTicks(1510) },
                    { new Guid("8cd77e0d-784b-4440-80e9-cd889395a420"), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), 5.7000000000000002, new DateTime(2019, 11, 4, 16, 51, 2, 149, DateTimeKind.Local).AddTicks(1272), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), "0612345678", new DateTime(2019, 11, 4, 17, 7, 2, 149, DateTimeKind.Local).AddTicks(1277), new Guid("546210b1-6ec8-4d64-b058-b3fa7e0740aa"), new DateTime(2019, 11, 4, 18, 51, 2, 149, DateTimeKind.Local).AddTicks(1253), 1, 14.26, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), 4, null, 4, 13.6, new DateTime(2019, 11, 4, 16, 21, 2, 149, DateTimeKind.Local).AddTicks(1262), new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), new DateTime(2019, 11, 4, 16, 42, 2, 149, DateTimeKind.Local).AddTicks(1267) },
                    { new Guid("3558b0eb-af94-4e37-b94a-8d82500cca37"), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), 5.7000000000000002, new DateTime(2019, 11, 4, 16, 51, 2, 149, DateTimeKind.Local).AddTicks(2361), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), "0612345678", new DateTime(2019, 11, 4, 16, 55, 2, 149, DateTimeKind.Local).AddTicks(2367), new Guid("f2f458f3-819e-46b3-be44-554f6a2fad93"), new DateTime(2019, 11, 4, 18, 51, 2, 149, DateTimeKind.Local).AddTicks(2343), 1, 14.26, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), 4, null, 4, 13.6, new DateTime(2019, 11, 4, 16, 21, 2, 149, DateTimeKind.Local).AddTicks(2351), new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), new DateTime(2019, 11, 4, 16, 42, 2, 149, DateTimeKind.Local).AddTicks(2356) },
                    { new Guid("7cb4d484-5fbc-4098-a4f5-bccfa1dd4a4a"), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), 5.7000000000000002, new DateTime(2019, 11, 4, 16, 51, 2, 149, DateTimeKind.Local).AddTicks(190), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), "0612345678", new DateTime(2019, 11, 4, 16, 52, 2, 149, DateTimeKind.Local).AddTicks(194), new Guid("fa8ec550-2863-4f01-82df-488d66004a60"), new DateTime(2019, 11, 4, 18, 51, 2, 149, DateTimeKind.Local).AddTicks(176), 1, 14.26, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), 4, null, 4, 13.6, new DateTime(2019, 11, 4, 16, 21, 2, 149, DateTimeKind.Local).AddTicks(184), new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), new DateTime(2019, 11, 4, 16, 42, 2, 149, DateTimeKind.Local).AddTicks(187) },
                    { new Guid("061d5bc1-62f3-4372-b354-77a4b8be26c0"), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), 5.7000000000000002, new DateTime(2019, 11, 4, 16, 51, 2, 149, DateTimeKind.Local).AddTicks(448), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), "0612345678", new DateTime(2019, 11, 4, 16, 52, 2, 149, DateTimeKind.Local).AddTicks(452), new Guid("fa8ec550-2863-4f01-82df-488d66004a60"), new DateTime(2019, 11, 4, 18, 51, 2, 149, DateTimeKind.Local).AddTicks(429), 1, 14.26, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), 4, 1.0, 4, 13.6, new DateTime(2019, 11, 4, 16, 21, 2, 149, DateTimeKind.Local).AddTicks(436), new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), new DateTime(2019, 11, 4, 16, 42, 2, 149, DateTimeKind.Local).AddTicks(443) },
                    { new Guid("54b90b3c-f3e9-47cc-a507-7e0a8fea7fb0"), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), 5.7000000000000002, new DateTime(2019, 11, 4, 16, 51, 2, 149, DateTimeKind.Local).AddTicks(2670), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), "0612345678", new DateTime(2019, 11, 4, 16, 59, 2, 149, DateTimeKind.Local).AddTicks(2676), new Guid("f2f458f3-819e-46b3-be44-554f6a2fad93"), new DateTime(2019, 11, 4, 18, 51, 2, 149, DateTimeKind.Local).AddTicks(2649), 1, 14.26, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), 4, null, 4, 13.6, new DateTime(2019, 11, 4, 16, 21, 2, 149, DateTimeKind.Local).AddTicks(2658), new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), new DateTime(2019, 11, 4, 16, 42, 2, 149, DateTimeKind.Local).AddTicks(2664) },
                    { new Guid("1e1909e9-de01-4221-bea1-80198de9254d"), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), 5.7000000000000002, new DateTime(2019, 11, 4, 16, 51, 2, 148, DateTimeKind.Local).AddTicks(9862), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), "0612345678", new DateTime(2019, 11, 4, 16, 51, 2, 148, DateTimeKind.Local).AddTicks(9866), new Guid("77f3c372-e321-4263-ba7a-0f86ce51de56"), new DateTime(2019, 11, 4, 18, 51, 2, 148, DateTimeKind.Local).AddTicks(9845), 1, 14.26, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), 4, 2.5, 4, 13.6, new DateTime(2019, 11, 4, 16, 21, 2, 148, DateTimeKind.Local).AddTicks(9852), new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), new DateTime(2019, 11, 4, 16, 42, 2, 148, DateTimeKind.Local).AddTicks(9859) },
                    { new Guid("3efb4583-bde7-4038-93ef-c7d30b845dd0"), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), 5.7000000000000002, new DateTime(2019, 11, 4, 16, 51, 2, 148, DateTimeKind.Local).AddTicks(9235), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), "0612345678", new DateTime(2019, 11, 4, 16, 50, 2, 148, DateTimeKind.Local).AddTicks(9241), new Guid("77f3c372-e321-4263-ba7a-0f86ce51de56"), new DateTime(2019, 11, 4, 18, 51, 2, 148, DateTimeKind.Local).AddTicks(9208), 1, 14.26, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), 4, null, 4, 13.6, new DateTime(2019, 11, 4, 16, 21, 2, 148, DateTimeKind.Local).AddTicks(9224), new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), new DateTime(2019, 11, 4, 16, 42, 2, 148, DateTimeKind.Local).AddTicks(9229) },
                    { new Guid("9c90af37-6ead-4b3c-ba11-d1c2e8634bba"), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), 5.7000000000000002, new DateTime(2019, 11, 4, 16, 51, 2, 149, DateTimeKind.Local).AddTicks(2926), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), "0612345678", new DateTime(2019, 11, 4, 16, 59, 2, 149, DateTimeKind.Local).AddTicks(2930), new Guid("1566979b-9fa6-4a51-8e67-08509ce79106"), new DateTime(2019, 11, 4, 18, 51, 2, 149, DateTimeKind.Local).AddTicks(2909), 1, 14.26, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), 4, null, 4, 13.6, new DateTime(2019, 11, 4, 16, 21, 2, 149, DateTimeKind.Local).AddTicks(2918), new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), new DateTime(2019, 11, 4, 16, 42, 2, 149, DateTimeKind.Local).AddTicks(2922) },
                    { new Guid("13aeefcd-305e-46d6-a1b8-8afaf8e016b7"), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), 5.7000000000000002, new DateTime(2019, 11, 4, 16, 51, 2, 149, DateTimeKind.Local).AddTicks(3145), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), "0612345678", new DateTime(2019, 11, 4, 16, 59, 2, 149, DateTimeKind.Local).AddTicks(3150), new Guid("b97b134c-9f3d-4bef-9085-85b16b2f0cff"), new DateTime(2019, 11, 4, 18, 51, 2, 149, DateTimeKind.Local).AddTicks(3130), 1, 14.26, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), 4, null, 4, 13.6, new DateTime(2019, 11, 4, 16, 21, 2, 149, DateTimeKind.Local).AddTicks(3136), new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), new DateTime(2019, 11, 4, 16, 42, 2, 149, DateTimeKind.Local).AddTicks(3140) },
                    { new Guid("8ea653a7-f384-406c-9e14-994e1bae27b4"), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), 5.7000000000000002, new DateTime(2019, 11, 4, 16, 51, 2, 149, DateTimeKind.Local).AddTicks(3399), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), "0612345678", new DateTime(2019, 11, 4, 16, 59, 2, 149, DateTimeKind.Local).AddTicks(3404), new Guid("03ab5306-01a8-400d-a43a-2edef178a187"), new DateTime(2019, 11, 4, 18, 51, 2, 149, DateTimeKind.Local).AddTicks(3382), 1, 14.26, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), 4, null, 4, 13.6, new DateTime(2019, 11, 4, 16, 21, 2, 149, DateTimeKind.Local).AddTicks(3389), new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), new DateTime(2019, 11, 4, 16, 42, 2, 149, DateTimeKind.Local).AddTicks(3393) },
                    { new Guid("7d5dd904-43ab-4ee4-9b72-78ec0caf629b"), new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), 5.7000000000000002, new DateTime(2019, 11, 4, 17, 0, 2, 145, DateTimeKind.Local).AddTicks(8072), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), "0612345678", null, new Guid("a6578db0-5e35-4a6b-967b-2d546e16b6f7"), new DateTime(2019, 11, 4, 18, 51, 2, 145, DateTimeKind.Local).AddTicks(7988), 0, 14.26, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), 3, null, 4, 13.6, new DateTime(2019, 11, 4, 16, 30, 2, 145, DateTimeKind.Local).AddTicks(8032), new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), new DateTime(2019, 11, 4, 16, 51, 2, 145, DateTimeKind.Local).AddTicks(8066) },
                    { new Guid("01b0fcd5-ecac-4158-972f-c13650563e7d"), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), 5.7000000000000002, new DateTime(2019, 11, 4, 16, 51, 2, 149, DateTimeKind.Local).AddTicks(3722), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), "0612345678", new DateTime(2019, 11, 4, 16, 59, 2, 149, DateTimeKind.Local).AddTicks(3727), new Guid("1c8d26ac-0879-4bd4-aa45-ede7206c315d"), new DateTime(2019, 11, 4, 18, 51, 2, 149, DateTimeKind.Local).AddTicks(3702), 1, 14.26, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), 4, null, 4, 13.6, new DateTime(2019, 11, 4, 16, 21, 2, 149, DateTimeKind.Local).AddTicks(3711), new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), new DateTime(2019, 11, 4, 16, 42, 2, 149, DateTimeKind.Local).AddTicks(3717) },
                    { new Guid("819363c4-434f-4934-bc2b-1d7de4ddef7a"), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), 5.7000000000000002, new DateTime(2019, 11, 4, 16, 51, 2, 149, DateTimeKind.Local).AddTicks(693), new Guid("f80cec78-70c7-43fa-be47-2f200e43eedf"), "0612345678", new DateTime(2019, 11, 4, 17, 49, 2, 149, DateTimeKind.Local).AddTicks(697), new Guid("c26682bd-d8bf-4698-a00e-02d059a2498d"), new DateTime(2019, 11, 4, 18, 51, 2, 149, DateTimeKind.Local).AddTicks(672), 1, 14.26, new Guid("d163b296-fc5c-4f1a-b56b-9fae13640a37"), 4, null, 4, 13.6, new DateTime(2019, 11, 4, 16, 21, 2, 149, DateTimeKind.Local).AddTicks(680), new Guid("bf78d3d2-cbc0-48a8-97f6-d0649e2408e3"), new DateTime(2019, 11, 4, 16, 42, 2, 149, DateTimeKind.Local).AddTicks(687) }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "AcceptedAt", "CreatedAt", "DelivererId", "DeliveryId", "ExpiredAt", "RefusedAt", "Status" },
                values: new object[] { new Guid("ef93546b-9d74-4862-b746-7dfdd15ba844"), null, new DateTime(2019, 11, 4, 16, 51, 2, 150, DateTimeKind.Local).AddTicks(4339), new Guid("b97b134c-9f3d-4bef-9085-85b16b2f0cff"), new Guid("082f1eda-2985-4d2b-8add-80487f8edf66"), new DateTime(2020, 11, 3, 16, 51, 2, 150, DateTimeKind.Local).AddTicks(4358), null, 0 });

            migrationBuilder.InsertData(
                table: "Feedback",
                columns: new[] { "Id", "Category", "DelivererId", "DeliveryId", "Rating" },
                values: new object[,]
                {
                    { new Guid("25c0ecf2-fc31-4576-a3b5-605a0cb5581b"), 0, new Guid("1c8d26ac-0879-4bd4-aa45-ede7206c315d"), new Guid("01b0fcd5-ecac-4158-972f-c13650563e7d"), 5 },
                    { new Guid("65b6b5b4-6af2-45ee-bcd1-7473777b3eef"), 2, new Guid("1c8d26ac-0879-4bd4-aa45-ede7206c315d"), new Guid("01b0fcd5-ecac-4158-972f-c13650563e7d"), 5 },
                    { new Guid("f44cf6b9-a710-4ac1-ac14-379f404a6dfc"), 1, new Guid("1c8d26ac-0879-4bd4-aa45-ede7206c315d"), new Guid("01b0fcd5-ecac-4158-972f-c13650563e7d"), 3 }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "AcceptedAt", "CreatedAt", "DelivererId", "DeliveryId", "ExpiredAt", "RefusedAt", "Status" },
                values: new object[,]
                {
                    { new Guid("f7bdd7eb-0c33-4f13-8a2a-9667e4c49152"), new DateTime(2019, 11, 4, 16, 59, 2, 151, DateTimeKind.Local).AddTicks(7087), new DateTime(2019, 11, 4, 16, 51, 2, 151, DateTimeKind.Local).AddTicks(7080), new Guid("5fc08418-6e23-4de8-88b0-1c513ddaf40d"), new Guid("4aea94b7-a21f-472f-be50-154fead4ac8c"), new DateTime(2019, 11, 4, 17, 1, 2, 151, DateTimeKind.Local).AddTicks(7092), null, 1 },
                    { new Guid("c89945f3-dfcd-491c-bee4-c752279e61a7"), null, new DateTime(2019, 11, 4, 16, 51, 2, 151, DateTimeKind.Local).AddTicks(6832), new Guid("546210b1-6ec8-4d64-b058-b3fa7e0740aa"), new Guid("4aea94b7-a21f-472f-be50-154fead4ac8c"), new DateTime(2019, 11, 4, 17, 1, 2, 151, DateTimeKind.Local).AddTicks(6843), new DateTime(2019, 11, 4, 16, 54, 2, 151, DateTimeKind.Local).AddTicks(6838), 2 },
                    { new Guid("2fc78d41-a4ee-42c4-be96-e222aca5db1c"), new DateTime(2019, 11, 4, 16, 59, 2, 151, DateTimeKind.Local).AddTicks(6935), new DateTime(2019, 11, 4, 16, 51, 2, 151, DateTimeKind.Local).AddTicks(6928), new Guid("5fc08418-6e23-4de8-88b0-1c513ddaf40d"), new Guid("c4f626f8-e626-45b9-8548-b1d922a49e0d"), new DateTime(2019, 11, 4, 17, 1, 2, 151, DateTimeKind.Local).AddTicks(6940), null, 1 },
                    { new Guid("ef398353-f31d-4503-9042-0f6429a0e6f1"), new DateTime(2019, 11, 4, 16, 53, 2, 151, DateTimeKind.Local).AddTicks(6745), new DateTime(2019, 11, 4, 16, 51, 2, 151, DateTimeKind.Local).AddTicks(6739), new Guid("546210b1-6ec8-4d64-b058-b3fa7e0740aa"), new Guid("c4f626f8-e626-45b9-8548-b1d922a49e0d"), new DateTime(2019, 11, 4, 17, 1, 2, 151, DateTimeKind.Local).AddTicks(6751), null, 1 },
                    { new Guid("6377aa78-8601-4000-af20-d91ce8d34638"), new DateTime(2019, 11, 4, 16, 53, 2, 151, DateTimeKind.Local).AddTicks(6641), new DateTime(2019, 11, 4, 16, 51, 2, 151, DateTimeKind.Local).AddTicks(6634), new Guid("546210b1-6ec8-4d64-b058-b3fa7e0740aa"), new Guid("8800377f-f8c9-4db8-8fed-60b4fa2b4bb6"), new DateTime(2019, 11, 4, 17, 1, 2, 151, DateTimeKind.Local).AddTicks(6646), null, 1 },
                    { new Guid("4d71bdb6-f607-41be-b3a4-59139b8a9a5c"), null, new DateTime(2019, 11, 4, 16, 51, 2, 151, DateTimeKind.Local).AddTicks(6532), new Guid("c26682bd-d8bf-4698-a00e-02d059a2498d"), new Guid("8800377f-f8c9-4db8-8fed-60b4fa2b4bb6"), new DateTime(2019, 11, 4, 17, 1, 2, 151, DateTimeKind.Local).AddTicks(6542), new DateTime(2019, 11, 4, 16, 53, 2, 151, DateTimeKind.Local).AddTicks(6538), 2 },
                    { new Guid("53580fad-c009-4c5f-928c-5d9808691616"), null, new DateTime(2019, 11, 4, 16, 51, 2, 151, DateTimeKind.Local).AddTicks(6209), new Guid("c26682bd-d8bf-4698-a00e-02d059a2498d"), new Guid("8cd77e0d-784b-4440-80e9-cd889395a420"), new DateTime(2019, 11, 4, 17, 1, 2, 151, DateTimeKind.Local).AddTicks(6222), new DateTime(2019, 11, 4, 16, 52, 2, 151, DateTimeKind.Local).AddTicks(6216), 2 },
                    { new Guid("f31dfe65-9b15-4ac1-9c9c-57cdc01f8d88"), new DateTime(2019, 11, 4, 16, 53, 2, 151, DateTimeKind.Local).AddTicks(6117), new DateTime(2019, 11, 4, 16, 51, 2, 151, DateTimeKind.Local).AddTicks(6112), new Guid("c26682bd-d8bf-4698-a00e-02d059a2498d"), new Guid("0415d421-3359-4117-a697-092186ae2569"), new DateTime(2019, 11, 4, 17, 1, 2, 151, DateTimeKind.Local).AddTicks(6121), null, 1 },
                    { new Guid("6dfa1e56-234d-4b4e-9ff6-b78447b6f27a"), new DateTime(2019, 11, 4, 16, 58, 2, 151, DateTimeKind.Local).AddTicks(5928), new DateTime(2019, 11, 4, 16, 51, 2, 151, DateTimeKind.Local).AddTicks(5922), new Guid("fa8ec550-2863-4f01-82df-488d66004a60"), new Guid("061d5bc1-62f3-4372-b354-77a4b8be26c0"), new DateTime(2019, 11, 4, 17, 1, 2, 151, DateTimeKind.Local).AddTicks(5932), null, 1 },
                    { new Guid("f5b62a35-fa21-4660-bf57-e30df51a2ff2"), new DateTime(2019, 11, 4, 17, 0, 2, 151, DateTimeKind.Local).AddTicks(7198), new DateTime(2019, 11, 4, 16, 51, 2, 151, DateTimeKind.Local).AddTicks(7192), new Guid("f2f458f3-819e-46b3-be44-554f6a2fad93"), new Guid("3558b0eb-af94-4e37-b94a-8d82500cca37"), new DateTime(2019, 11, 4, 17, 1, 2, 151, DateTimeKind.Local).AddTicks(7203), null, 1 },
                    { new Guid("8370e546-5bf9-4f95-9e8c-d50adc51e202"), new DateTime(2019, 11, 4, 16, 59, 2, 151, DateTimeKind.Local).AddTicks(5728), new DateTime(2019, 11, 4, 16, 51, 2, 151, DateTimeKind.Local).AddTicks(5720), new Guid("fa8ec550-2863-4f01-82df-488d66004a60"), new Guid("7cb4d484-5fbc-4098-a4f5-bccfa1dd4a4a"), new DateTime(2019, 11, 4, 17, 1, 2, 151, DateTimeKind.Local).AddTicks(5732), null, 1 },
                    { new Guid("0498ee91-92bc-408a-8a7a-18349769601a"), new DateTime(2019, 11, 4, 16, 58, 2, 151, DateTimeKind.Local).AddTicks(5604), new DateTime(2019, 11, 4, 16, 51, 2, 151, DateTimeKind.Local).AddTicks(5595), new Guid("77f3c372-e321-4263-ba7a-0f86ce51de56"), new Guid("1e1909e9-de01-4221-bea1-80198de9254d"), new DateTime(2019, 11, 4, 17, 1, 2, 151, DateTimeKind.Local).AddTicks(5611), null, 1 },
                    { new Guid("832b443c-909e-4b27-8059-2ec7894a9337"), new DateTime(2019, 11, 4, 16, 59, 2, 151, DateTimeKind.Local).AddTicks(5106), new DateTime(2019, 11, 4, 16, 51, 2, 151, DateTimeKind.Local).AddTicks(5089), new Guid("77f3c372-e321-4263-ba7a-0f86ce51de56"), new Guid("3efb4583-bde7-4038-93ef-c7d30b845dd0"), new DateTime(2019, 11, 4, 17, 1, 2, 151, DateTimeKind.Local).AddTicks(5114), null, 1 },
                    { new Guid("be512cf1-8c41-467d-b34f-e02088eb9f4d"), new DateTime(2019, 11, 4, 17, 0, 2, 151, DateTimeKind.Local).AddTicks(7676), new DateTime(2019, 11, 4, 16, 51, 2, 151, DateTimeKind.Local).AddTicks(7665), new Guid("1566979b-9fa6-4a51-8e67-08509ce79106"), new Guid("9c90af37-6ead-4b3c-ba11-d1c2e8634bba"), new DateTime(2019, 11, 4, 17, 1, 2, 151, DateTimeKind.Local).AddTicks(7681), null, 1 },
                    { new Guid("b84e4c03-339b-4ce2-81a6-e9176ecbad92"), new DateTime(2019, 11, 4, 17, 0, 2, 151, DateTimeKind.Local).AddTicks(7577), new DateTime(2019, 11, 4, 16, 51, 2, 151, DateTimeKind.Local).AddTicks(7571), new Guid("03ab5306-01a8-400d-a43a-2edef178a187"), new Guid("8ea653a7-f384-406c-9e14-994e1bae27b4"), new DateTime(2019, 11, 4, 17, 1, 2, 151, DateTimeKind.Local).AddTicks(7583), null, 1 },
                    { new Guid("6f13c993-b14b-4ef0-9515-98aa1830435c"), new DateTime(2019, 11, 4, 17, 0, 2, 151, DateTimeKind.Local).AddTicks(7400), new DateTime(2019, 11, 4, 16, 51, 2, 151, DateTimeKind.Local).AddTicks(7393), new Guid("a6578db0-5e35-4a6b-967b-2d546e16b6f7"), new Guid("7d5dd904-43ab-4ee4-9b72-78ec0caf629b"), new DateTime(2019, 11, 4, 17, 1, 2, 151, DateTimeKind.Local).AddTicks(7403), null, 1 },
                    { new Guid("118ce4db-2629-4417-ab31-b8b355f24fdd"), new DateTime(2019, 11, 4, 17, 0, 2, 151, DateTimeKind.Local).AddTicks(7478), new DateTime(2019, 11, 4, 16, 51, 2, 151, DateTimeKind.Local).AddTicks(7472), new Guid("1c8d26ac-0879-4bd4-aa45-ede7206c315d"), new Guid("01b0fcd5-ecac-4158-972f-c13650563e7d"), new DateTime(2019, 11, 4, 17, 1, 2, 151, DateTimeKind.Local).AddTicks(7484), null, 1 },
                    { new Guid("feabedb2-b543-48a9-9bc3-c8fc13111308"), new DateTime(2019, 11, 4, 16, 52, 2, 151, DateTimeKind.Local).AddTicks(6027), new DateTime(2019, 11, 4, 16, 51, 2, 151, DateTimeKind.Local).AddTicks(6022), new Guid("c26682bd-d8bf-4698-a00e-02d059a2498d"), new Guid("819363c4-434f-4934-bc2b-1d7de4ddef7a"), new DateTime(2019, 11, 4, 17, 1, 2, 151, DateTimeKind.Local).AddTicks(6031), null, 1 },
                    { new Guid("60ecff74-32ca-4d29-b5c4-a03b0148825c"), new DateTime(2019, 11, 4, 17, 0, 2, 151, DateTimeKind.Local).AddTicks(7302), new DateTime(2019, 11, 4, 16, 51, 2, 151, DateTimeKind.Local).AddTicks(7295), new Guid("f2f458f3-819e-46b3-be44-554f6a2fad93"), new Guid("54b90b3c-f3e9-47cc-a507-7e0a8fea7fb0"), new DateTime(2019, 11, 4, 17, 1, 2, 151, DateTimeKind.Local).AddTicks(7309), null, 1 }
                });*/

            migrationBuilder.CreateIndex(
                name: "IX_Availabilities_DelivererId",
                table: "Availabilities",
                column: "DelivererId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_CurrentId",
                table: "Deliveries",
                column: "CurrentId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_CustomerId",
                table: "Deliveries",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_DelivererId",
                table: "Deliveries",
                column: "DelivererId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_WarehouseId",
                table: "Deliveries",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_DelivererId",
                table: "Feedback",
                column: "DelivererId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_DeliveryId",
                table: "Feedback",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_DelivererId",
                table: "Locations",
                column: "DelivererId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_DelivererId",
                table: "Notifications",
                column: "DelivererId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_DeliveryId",
                table: "Notifications",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_HomeId",
                table: "Users",
                column: "HomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Token",
                table: "Users",
                column: "Token",
                unique: true,
                filter: "[Token] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_Users_DelivererId",
                table: "Feedback",
                column: "DelivererId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_Deliveries_DeliveryId",
                table: "Feedback",
                column: "DeliveryId",
                principalTable: "Deliveries",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_DelivererId",
                table: "Notifications",
                column: "DelivererId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Deliveries_DeliveryId",
                table: "Notifications",
                column: "DeliveryId",
                principalTable: "Deliveries",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Users_DelivererId",
                table: "Deliveries",
                column: "DelivererId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Locations_CurrentId",
                table: "Deliveries",
                column: "CurrentId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Locations_CustomerId",
                table: "Deliveries",
                column: "CustomerId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Locations_WarehouseId",
                table: "Deliveries",
                column: "WarehouseId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Locations_HomeId",
                table: "Users",
                column: "HomeId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Users_DelivererId",
                table: "Locations");

            migrationBuilder.DropTable(
                name: "Availabilities");

            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Deliveries");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
