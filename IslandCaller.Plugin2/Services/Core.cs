using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IslandCaller.Plugin2.Services
{
    public class CoreService(ProfileService profileService, HistoryService historyService, ILogger<CoreService> logger)
    {
        private readonly ProfileService profileService = profileService;
        private readonly HistoryService historyService = historyService;
        private readonly ILogger<CoreService> logger = logger;
        internal class Person
        {
            internal int Id { get; set; }
            internal string Name { get; set; }
            internal int Gender { get; set; }
            internal double ManualWeight { get; set; } = 1.0; // 教师设置的基础权重，默认为 1.0
            internal double Weight { get; set; }
        }
        // 计算学生被点名的权重
        internal List<Person> Persons { get; set; } = new List<Person>();

        internal void InitializeCore()
        {
            logger.LogInformation("初始化 Core 模块，加载学生信息...");
            foreach (var person in profileService.Members)
            {
                Persons.Add(new Person
                {
                    Id = person.Id,
                    Name = person.Name,
                    Gender = person.Gender,
                    ManualWeight = person.ManualWeight,
                    Weight = 0.0
                });
            }
            ComputeWeightsForAllStudents();
        }

        private double ComputeSingleWeight(
                                double manualWeight,     // W_manual_i
                                int lastHitStep,         // s_i_last：该学生上次被点到的轮次（没点过可设为 -1）
                                int nHist,               // n_hist_i：历史被点次数
                                double avgHist)          // avg_hist：全班历史平均被点次数
        {
            // -----------------------------
            // 1. 本节课防重复因子（随时间恢复）
            // -----------------------------
            const double fMin = 0.5;     // 最低值
            const double beta = 0.54;    // 恢复速度参数（推荐值）

            int deltaS = lastHitStep;
            if (deltaS < 0) deltaS = 0;

            // F_session = 1 - (1 - fMin) * exp(-beta * Δs)
            double F_session = 1 - (1 - fMin) * Math.Exp(-beta * deltaS);

            // -----------------------------
            // 2. 历史均衡因子
            // -----------------------------
            const double eps = 1.0;      // 平滑项
            const double gamma = 0.7;    // 补偿强度
            const double rMin = 0.6;     // 最小补偿
            const double rMax = 1.6;     // 最大补偿

            // F_history = clip( ((avgHist+1)/(nHist+1))^gamma , rMin, rMax )
            double ratio = (avgHist + eps) / (nHist + eps);
            double F_history = Math.Pow(ratio, gamma);
            F_history = Math.Max(rMin, Math.Min(rMax, F_history));

            // -----------------------------
            // 3. 最终权重
            // -----------------------------
            return manualWeight * F_session * F_history;
        }

        private void ComputeWeightsForAllStudents()
        {
            // 计算全班历史平均被点次数
            double avgHist = historyService.GetAverageLongTermCount();
            logger.LogTrace($"计算全班历史平均被点次数: {avgHist}");
            foreach (var person in Persons)
            {
                int nHist = historyService.GetLongTermCount(person.Name);
                int lastHitStep = historyService.GetLastCallIndex(person.Name);
                double weight = ComputeSingleWeight(
                                    person.ManualWeight,
                                    lastHitStep,
                                    nHist,
                                    avgHist);
                person.Weight = weight;
                logger.LogTrace($"计算权重 - 学生: {person.Name}, ManualWeight: {person.ManualWeight}, LastHitStep: {lastHitStep}, nHist: {nHist}, Weight: {weight}");
            }
        }

        internal string GetRandomStudent()
        {
            // 计算权重总和
            double totalWeight = Persons.Sum(p => p.Weight);
            logger.LogTrace($"计算权重总和: {totalWeight}");
            if (totalWeight == 0) return "Error"; // 避免除以零
            // 生成一个 [0, totalWeight) 的随机数
            Random rand = new Random();
            double r = rand.NextDouble() * totalWeight;
            logger.LogTrace($"生成随机数: {r} (范围: [0, {totalWeight}))");
            // 根据权重选择学生
            double cumulative = 0;
            foreach (var person in Persons)
            {
                cumulative += person.Weight;
                if (r < cumulative)
                {
                    historyService.Add(person.Name);
                    logger.LogTrace($"抽取到学生：{person.Name}");
                    ComputeWeightsForAllStudents();
                    return person.Name;
                }
            }
            logger.LogWarning($"随机选择学生时发生了意外情况，权重总和: {totalWeight}, 随机数: {r}");
            return "Error"; // 理论上不应该到达这里
        }
    }
}

