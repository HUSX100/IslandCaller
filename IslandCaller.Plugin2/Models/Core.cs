using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IslandCaller.Models
{
    internal class Core
    {
        public double ComputeWeight(
                                double manualWeight,     // W_manual_i
                                int currentStep,         // s：当前是本节课第几次抽取
                                int lastHitStep,         // s_i_last：该学生上次被点到的轮次（没点过可设为 -1）
                                int nHist,               // n_hist_i：历史被点次数
                                double avgHist)          // avg_hist：全班历史平均被点次数
        {
            // -----------------------------
            // 1. 本节课防重复因子（随时间恢复）
            // -----------------------------
            const double fMin = 0.5;     // 最低值
            const double beta = 0.54;    // 恢复速度参数（推荐值）

            int deltaS = (lastHitStep < 0) ? int.MaxValue : (currentStep - lastHitStep);
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
    }
}
