using PPCMD.Data;
using PPCMD.Models;
using Microsoft.EntityFrameworkCore;

namespace PPCMD.utils
{
    public class CompanySeeder
    {
        private readonly ApplicationDbContext _context;

        public CompanySeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedDutyTypesForCompanyAsync(int companyId)
        {
            // Only add if not already seeded
            var existing = await _context.DutyTypes
                .Where(d => d.CompanyId == companyId)
                .AnyAsync();

            if (existing) return;

            var dutyTypes = new List<DutyType>
        {
            new DutyType { Name = "Custom Duty", Description = "Base import duty", CompanyId = companyId },
            new DutyType { Name = "Additional Custom Duty", Description = "Additional custom duty rate", CompanyId = companyId },
            new DutyType { Name = "Regulatory Duty", Description = "Regulatory duty imposed by government", CompanyId = companyId },
            new DutyType { Name = "Sales Tax", Description = "General sales tax", CompanyId = companyId },
            new DutyType { Name = "Additional Sales Tax", Description = "Extra sales tax on certain items", CompanyId = companyId },
            new DutyType { Name = "Income Tax", Description = "Advance income tax on imports", CompanyId = companyId },
            new DutyType { Name = "Additional Income Tax", Description = "Advance income tax on imports", CompanyId = companyId },
            new DutyType { Name = "PSQC", Description = "Pakistan Standards & Quality Control fee", CompanyId = companyId },
            new DutyType { Name = "Wharfage", Description = "Port wharfage charges", CompanyId = companyId }
        };

            _context.DutyTypes.AddRange(dutyTypes);
            await _context.SaveChangesAsync();
        }

        public async Task SeedClientTypesForCompanyAsync(int companyId)
        {
            // Only add if not already seeded
            var existing = await _context.ClientTypes
                .Where(ct => ct.CompanyId == companyId)
                .AnyAsync();
            if (existing) return;
            var clientTypes = new List<ClientType>
        {
            new ClientType { Name = "Commercial",  CompanyId = companyId },
            new ClientType { Name = "Industrial",  CompanyId = companyId },
            new ClientType { Name = "Government",  CompanyId = companyId },
            new ClientType { Name = "Individual",  CompanyId = companyId },
            new ClientType { Name = "Others",  CompanyId = companyId }
        };
            _context.ClientTypes.AddRange(clientTypes);
            await _context.SaveChangesAsync();
        }


        public async Task SeedClientsForCompanyAsync(int companyId)
        {
            // Get ClientTypes for this company
            var clientTypes = await _context.ClientTypes
                .Where(ct => ct.CompanyId == companyId)
                .ToListAsync();

            if (!clientTypes.Any()) return;

            // Only seed if no clients exist yet
            var existing = await _context.Clients
                .Where(c => c.CompanyId == companyId)
                .AnyAsync();
            if (existing) return;

            var rnd = new Random();

            var pakCities = new[] { "Karachi", "Lahore", "Islamabad", "Faisalabad", "Multan", "Quetta", "Peshawar" };

            var clients = new List<Client>();

            for (int i = 1; i <= 15; i++)
            {
                var type = (i % 2 == 0)
                    ? clientTypes.First(ct => ct.Name == "Commercial")
                    : clientTypes.First(ct => ct.Name == "Industrial");

                var client = new Client
                {
                    ClientName = $"Client {i} Pvt Ltd",
                    ContactPerson = $"Contact Person {i}",
                    Phone = $"021-{rnd.Next(3000000, 3999999)}",
                    Mobile = $"03{rnd.Next(0, 9)}-{rnd.Next(1000000, 9999999)}",
                    Address = $"{rnd.Next(10, 200)} {pakCities[rnd.Next(pakCities.Length)]}, Pakistan",
                    GST = $"07{i}{rnd.Next(1000, 9999)}",
                    NTN = $"NTN{i}{rnd.Next(10000, 99999)}",
                    ClientTypeId = type.Id,
                    CompanyId = companyId,
                    CreatedAt = DateTime.UtcNow
                };

                // Add emails
                int emailCount = rnd.Next(1, 4); // 1–3 emails
                for (int e = 1; e <= emailCount; e++)
                {
                    client.Emails.Add(new ClientEmail
                    {
                        Email = $"client{i}.contact{e}@example.com",
                        CreatedAt = DateTime.UtcNow
                    });
                }

                clients.Add(client);
            }

            _context.Clients.AddRange(clients);
            await _context.SaveChangesAsync();
        }


        public async Task SeedLolosAsync(int companyId)
        {
            if (await _context.Lolos.AnyAsync(l => l.CompanyId == companyId))
                return;

            var lolos = new List<Lolo>
            {
                new Lolo { Name = "UOSL SHIPPING & LOGISTICS PVT LTD", ShortName = "UOSL", Phone = "123", Email = "", NTN = "", CompanyId = companyId },
                new Lolo { Name = "MEGATECH PVT LTD", ShortName = "MEGA", Phone = "323", Email = "", NTN = "", CompanyId = companyId },
                new Lolo { Name = "RAP TRADING", ShortName = "RAP", Phone = "0", Email = "", NTN = "", CompanyId = companyId },
                new Lolo { Name = "CARGO SUPPORT SERVICES PVT LTD", ShortName = "CARGO", Phone = "0", Email = "", NTN = "", CompanyId = companyId },
                new Lolo { Name = "A&A CARGO TRANSPORT", ShortName = "A&A", Phone = "0320-2717117", Email = "---", NTN = "", CompanyId = companyId }
            };

            _context.Lolos.AddRange(lolos);
            await _context.SaveChangesAsync();
        }

        public async Task SeedShippingLinesAsync(int companyId)
        {
            if (await _context.ShippingLines.AnyAsync(s => s.CompanyId == companyId))
                return;

            var shippingLines = new List<ShippingLine>
            {
                new ShippingLine { Name = "OCEAN NETWORK EXPRESS PAKISTAN PVT LTD", ShortName = "ONE", Phone = "021-111-111-663", Email = "pakistan.customercare@one-line.com", NTN = "", CompanyId = companyId },
                new ShippingLine { Name = "MSC AGENCY PAKISTAN PVT LTD", ShortName = "MSC", Phone = "021-35632001", Email = "pk532-impcounterkhi@msc.com", NTN = "", CompanyId = companyId },
                new ShippingLine { Name = "UNITED MARINE AGENCIES PVT LTD", ShortName = "UMA", Phone = "021-111-111-862", Email = "coms.pakistan@umapk.com", NTN = "", CompanyId = companyId },
                new ShippingLine { Name = "MAERSK PAKISTAN PVT LTD", ShortName = "MAERSK", Phone = "021-111-623-775", Email = "pkrefund@maersk.com", NTN = "", CompanyId = companyId },
                new ShippingLine { Name = "COSCO SHIPPING LINES PAKISTAN PVT LTD", ShortName = "COSCO", Phone = "021-35180630-34", Email = "cs.pakistan@coscon.com", NTN = "", CompanyId = companyId },
                new ShippingLine { Name = "OOCL PAKISTAN PVT LTD", ShortName = "OOCL", Phone = "021-35147989-99", Email = "karibdoc@oocl.com", NTN = "", CompanyId = companyId },
                new ShippingLine { Name = "GREENPAK SHIPPING PVT LTD", ShortName = "GREENPAK", Phone = "021-111-123-477", Email = "arsalan.ahmed@greenpakshipping.com", NTN = "", CompanyId = companyId },
                new ShippingLine { Name = "EAST WIND SHIPPING COMPANY PVT LTD", ShortName = "EAST WIND", Phone = "021-35670251-4", Email = "imp.csl@ews.com.pk,docs.csl@ews.com.pk", NTN = "", CompanyId = companyId },
                new ShippingLine { Name = "INTERNATIONAL LOGISTICS MASTER", ShortName = "ILM", Phone = "021-32423047", Email = "", NTN = "", CompanyId = companyId },
                new ShippingLine { Name = "SOUTHERN AGENCIES PVT LTD", ShortName = "SOUTHERN", Phone = "021-32857790", Email = "docimp1@sal-psg.com", NTN = "", CompanyId = companyId },
                new ShippingLine { Name = "RIAZEDA PVT LTD", ShortName = "RIAZEDA", Phone = "021-32401181-5", Email = "info@riazeda.com.pk", NTN = "", CompanyId = companyId },
                new ShippingLine { Name = "CMA CGM PAKISTAN PVT LTD", ShortName = "CMA", Phone = "021-35147810-20", Email = "kar.impcs@cma-cgm.com", NTN = "", CompanyId = companyId },
                new ShippingLine { Name = "ACTIVE FREIGHT SERVICES PVT LTD", ShortName = "ACTIVE FREIGHT", Phone = "021-34150621-24", Email = "import2@activefreightpak.com", NTN = "", CompanyId = companyId },
                new ShippingLine { Name = "INTERNATIONAL LOGISTICS MASTER", ShortName = "INTERNATIONAL", Phone = "021-2424247-8", Email = "abbas@ilm-pak.com", NTN = "", CompanyId = companyId },
                new ShippingLine { Name = "HAPAG LLOYD PAKISTAN PVT LTD", ShortName = "H L", Phone = "021-37133000", Email = "pakistan@service.hlag.com", NTN = "", CompanyId = companyId },
                new ShippingLine { Name = "SHIP NAUTICA PVT LTD", ShortName = "SHIP NAUTICA", Phone = "021-32601355-58-59", Email = "doc@shipnautica.com", NTN = "", CompanyId = companyId },
                new ShippingLine { Name = "DHL GLOBAL FORWARDING PAKISTAN PVT LTD", ShortName = "DHL", Phone = "021-111-345-111", Email = "", NTN = "", CompanyId = companyId },
                // ...continue adding all shipping lines from your SQL similarly
            };

            _context.ShippingLines.AddRange(shippingLines);
            await _context.SaveChangesAsync();
        }

        public async Task SeedTerminalsAsync(int companyId)
        {
            if (await _context.Terminals.AnyAsync(t => t.CompanyId == companyId))
                return;

            var terminals = new List<Terminal>
            {
                new Terminal { Name = "PAKISTAN INTERNATIONAL CONTAINER TERMINAL LTD", ShortName = "PICTL", Phone = "111 117 428", Email = "helpdesk@pict.com.pk", NTN = "", CompanyId = companyId },
                new Terminal { Name = "KARACHI INTERNATIONAL CONTAINER TERMINAL LTD", ShortName = "KICTL", Phone = "021-111-542-850", Email = "support@kictl.com,helpdesk@kictl.com", NTN = "", CompanyId = companyId },
                new Terminal { Name = "SOUTH ASIA PAKISTAN TERMINALS LTD", ShortName = "SAPTL", Phone = "32862771–76", Email = "info@sapt.com.pk", NTN = "", CompanyId = companyId },
                new Terminal { Name = "QASIM INTERNATIONAL CONTAINER TERMINAL LTD", ShortName = "QICTL", Phone = "111786888", Email = "qictcustomer.service@dpworld.com", NTN = "", CompanyId = companyId },
                new Terminal { Name = "AL HAMD INTERNATIONAL CONTAINER TERMINAL PVT LTD", ShortName = "AICTL", Phone = "111000176", Email = "alerts@aictpk.com", NTN = "", CompanyId = companyId },
                new Terminal { Name = "BURMA OIL MILLS LTD", ShortName = "BOML", Phone = "111111175", Email = "info@bifs.com.pk", NTN = "", CompanyId = companyId },
                new Terminal { Name = "BAY WEST PVT LTD", ShortName = "BAY WEST", Phone = "32330028, 32330031", Email = "", NTN = "", CompanyId = companyId },
                // ...continue adding all terminals from your SQL similarly
            };

            _context.Terminals.AddRange(terminals);
            await _context.SaveChangesAsync();
        }


        public async Task SeedCitiesAsync(int companyId)
        {
            if (await _context.Cities.AnyAsync(t => t.CompanyId == companyId))
                return;

            var cities = new List<City>
            {
                // Punjab (10 most popular)
                new City { Name = "Lahore", CompanyId = companyId },
                new City { Name = "Faisalabad", CompanyId = companyId },
                new City { Name = "Rawalpindi", CompanyId = companyId },
                new City { Name = "Multan", CompanyId = companyId },
                new City { Name = "Gujranwala", CompanyId = companyId },
                new City { Name = "Sargodha", CompanyId = companyId },
                new City { Name = "Sialkot", CompanyId = companyId },
                new City { Name = "Bahawalpur", CompanyId = companyId },
                new City { Name = "Sahiwal", CompanyId = companyId },
                new City { Name = "Jhang", CompanyId = companyId },
        
                // Sindh (10 most popular)
                new City { Name = "Karachi", CompanyId = companyId },
                new City { Name = "Hyderabad", CompanyId = companyId },
                new City { Name = "Sukkur", CompanyId = companyId },
                new City { Name = "Larkana", CompanyId = companyId },
                new City { Name = "Nawabshah", CompanyId = companyId },
                new City { Name = "Mirpur Khas", CompanyId = companyId },
                new City { Name = "Thatta", CompanyId = companyId },
                new City { Name = "Jacobabad", CompanyId = companyId },
                new City { Name = "Shikarpur", CompanyId = companyId },
                new City { Name = "Khairpur", CompanyId = companyId },
        
                // Khyber Pakhtunkhwa (10 most popular)
                new City { Name = "Peshawar", CompanyId = companyId },
                new City { Name = "Mardan", CompanyId = companyId },
                new City { Name = "Abbottabad", CompanyId = companyId },
                new City { Name = "Kohat", CompanyId = companyId },
                new City { Name = "Bannu", CompanyId = companyId },
                new City { Name = "Swabi", CompanyId = companyId },
                new City { Name = "Dera Ismail Khan", CompanyId = companyId },
                new City { Name = "Charsadda", CompanyId = companyId },
                new City { Name = "Nowshera", CompanyId = companyId },
                new City { Name = "Mansehra", CompanyId = companyId },
        
                // Balochistan (10 most popular)
                new City { Name = "Quetta", CompanyId = companyId },
                new City { Name = "Turbat", CompanyId = companyId },
                new City { Name = "Khuzdar", CompanyId = companyId },
                new City { Name = "Chaman", CompanyId = companyId },
                new City { Name = "Gwadar", CompanyId = companyId },
                new City { Name = "Zhob", CompanyId = companyId },
                new City { Name = "Dera Allah Yar", CompanyId = companyId },
                new City { Name = "Usta Muhammad", CompanyId = companyId },
                new City { Name = "Sibi", CompanyId = companyId },
                new City { Name = "Loralai", CompanyId = companyId },
        
                // Islamabad Capital Territory
                new City { Name = "Islamabad", CompanyId = companyId },
        
                // Azad Jammu & Kashmir (5 most popular)
                new City { Name = "Muzaffarabad", CompanyId = companyId },
                new City { Name = "Mirpur", CompanyId = companyId },
                new City { Name = "Rawalakot", CompanyId = companyId },
                new City { Name = "Kotli", CompanyId = companyId },
                new City { Name = "Bhimber", CompanyId = companyId },
        
                // Gilgit-Baltistan (5 most popular)
                new City { Name = "Gilgit", CompanyId = companyId },
                new City { Name = "Skardu", CompanyId = companyId },
                new City { Name = "Chilas", CompanyId = companyId },
                new City { Name = "Ghizer", CompanyId = companyId },
                new City { Name = "Hunza", CompanyId = companyId }
            };

            _context.Cities.AddRange(cities);
            await _context.SaveChangesAsync();
        }


        public async Task SeedPortsAsync(int companyId)
        {
            if (await _context.Ports.AnyAsync(p => p.CompanyId == companyId))
                return;

            var ports = new List<Port>
            {
                new Port {
                    Name = "Karachi Port Trust",
                    ShortName = "KPT",
                    Phone = "021-99212600-10",
                    Email = "info@kpt.gov.pk",
                    CompanyId = companyId
                },
                new Port {
                    Name = "Port Qasim Authority",
                    ShortName = "PQA",
                    Phone = "021-34730011-15",
                    Email = "info@portqasim.gov.pk",
                    CompanyId = companyId
                },
                new Port {
                    Name = "Gwadar Port Authority",
                    ShortName = "GPA",
                    Phone = "086-4210265",
                    Email = "info@gwadarport.gov.pk",
                    CompanyId = companyId
                },
                new Port {
                    Name = "Pakistan National Shipping Corporation",
                    ShortName = "PNSC",
                    Phone = "021-99206400-09",
                    Email = "info@pnsc.com.pk",
                    CompanyId = companyId
                },
                new Port {
                    Name = "Keti Bandar Port",
                    ShortName = "KBP",
                    Phone = "022-2630010",
                    Email = "admin@ketibandarport.gov.pk",
                    CompanyId = companyId
                },
                new Port {
                    Name = "Ormara Port",
                    ShortName = "OP",
                    Phone = "085-3310267",
                    Email = "ormaraport@navy.gov.pk",
                    CompanyId = companyId
                },
                new Port {
                    Name = "Pasni Port",
                    ShortName = "PP",
                    Phone = "086-4310265",
                    Email = "pasni.port@balochistan.gov.pk",
                    CompanyId = companyId
                },
                new Port {
                    Name = "Jiwani Port",
                    ShortName = "JP",
                    Phone = "086-4410234",
                    Email = "jiwani.port@gov.pk",
                    CompanyId = companyId
                }
            };

            _context.Ports.AddRange(ports);
            await _context.SaveChangesAsync();
        }

        public async Task SeedPayorderHeadersAsync(int companyId)
        {
            if (await _context.PayorderHeaders.AnyAsync(p => p.CompanyId == companyId))
                return;

            var headers = new List<PayorderHeader>
            {
                new PayorderHeader {
                    Name = "Custom Duty",
                    Description = "Collector Of Customs",
                    CompanyId = companyId
                },
                new PayorderHeader {
                    Name = "EXCISE DUTY",
                    Description = "EXCISE & TAXATION OFFICER SEA DUES",
                    CompanyId = companyId
                    },
                new PayorderHeader {
                    Name = "TERMINAL WHARFAGE",
                    Description = null,
                    CompanyId = companyId
                },
                new PayorderHeader {
                    Name = "DELIVERY ORDER",
                    Description = null,
                    CompanyId = companyId
                },
                new PayorderHeader {
                    Name = "CONTAINER RENT",
                    Description = null,
                    CompanyId = companyId
                },
                new PayorderHeader {
                    Name = "CONTAINER DEPOSIT",
                    Description = null,
                    CompanyId = companyId
                },
                new PayorderHeader {
                    Name = "PSQCA",
                    Description = "PSQCA SDC IMPORT & EXPORT",
                    CompanyId = companyId
                },
                new PayorderHeader {
                    Name = "PSQCA",
                    Description = "COMMERCID TESTING OTHER RELATED AT NED UET D-CPE",
                    CompanyId = companyId
                },
                new PayorderHeader {
                    Name = "PSQCA",
                    Description = "DIRECTOR QCC/PSQCA KARACHI",
                    CompanyId = companyId
                },
                new PayorderHeader {
                    Name = "TPL",
                    Description = "TPL TRAKKER LIMITED NTN # 7504487",
                    CompanyId = companyId
                },
                new PayorderHeader {
                    Name = "LIFT ON LIFT OFF",
                    Description = null,
                    CompanyId = companyId
                },
                new PayorderHeader {
                    Name = "CAA",
                    Description = "CIVIL AVIATION AUTHORITY",
                    CompanyId = companyId
                },
                new PayorderHeader {
                    Name = "ENDORSEMENT",
                    Description = null,
                    CompanyId = companyId
                },
                new PayorderHeader {
                    Name = "FRIEGHT",
                    Description = null,
                    CompanyId = companyId
                }

            };

            _context.PayorderHeaders.AddRange(headers);
            await _context.SaveChangesAsync();
        }
    }

}
