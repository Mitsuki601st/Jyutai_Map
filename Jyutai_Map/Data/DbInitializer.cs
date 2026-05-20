using Jyutai_Map.Models;

namespace Jyutai_Map.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.TrafficReports.Any())
            {
                return;   // DB has been seeded
            }

            var reports = new TrafficReport[]
            {
                new TrafficReport{Location="東京駅前", CongestionLevel=4, Description="工事のため混雑", ReportedAt=DateTime.UtcNow},
                new TrafficReport{Location="渋谷スクランブル交差点", CongestionLevel=5, Description="イベント開催中", ReportedAt=DateTime.UtcNow.AddMinutes(-30)},
                new TrafficReport{Location="新宿駅西口", CongestionLevel=2, Description="比較的スムーズ", ReportedAt=DateTime.UtcNow.AddHours(-1)},
                new TrafficReport{Location="六本木通り", CongestionLevel=3, Description="通常通りの交通量", ReportedAt=DateTime.UtcNow.AddMinutes(-15)}
            };

            foreach (var r in reports)
            {
                context.TrafficReports.Add(r);
            }
            context.SaveChanges();
        }
    }
}
