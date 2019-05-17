using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace WorkEvaluate.Layouts.WorkEvaluate.DAL
{
    public class Works
    {
        /// <summary>
        /// 获取每个作品的评分信息
        /// </summary>
        /// <param name="worksID"></param>
        /// <returns></returns>
        public static DataSet GetWorksExpertByWorksID(long worksID)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetWorksExpertByWorksID", worksID);
            return ds;
        }
        /// <summary>
        /// 获取期次下的学生作品
        /// </summary>
        /// <param name="periodID"></param>
        /// <returns></returns>
        public static DataSet GetWorksByPeriodID(long periodID)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetWorksByPeriodID", periodID);
            return ds;
        }
        public static DataSet GetWorksAllotTimesByWorsID(long WorksID)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetWorksAllotTimesByWorsID", WorksID);
            return ds;
        }
        /// <summary>
        /// 获取样例作品
        /// </summary>
        /// <param name="periodID"></param>
        /// <returns></returns>
        public static DataSet GetSampleWorksByPeriodID(long periodID)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetSampleWorksByPeriodID", periodID);
            return ds;

        }
       
        /// <summary>
        /// 通过期次和账户获取该用户在该期次中的作品
        /// </summary>
        /// <param name="periodID">期次ID</param>
        /// <param name="account">用户帐户名</param>
        /// <returns>DataSet</returns>
        public static DataSet GetWorksByPeriodAndAccount(long periodID,string account)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetWorksByAccount", periodID, account);
            return ds;
        }
        /// <summary>
        /// 获取UserID可以评审但还未分配完成的作品
        /// </summary>
        /// <param name="userId">评分人ID</param>
        /// <param name="periodId">当前期次ID</param>
        /// <param name="allotTimes">本期互评组内人数</param>
        /// <returns></returns>
        public static DataSet GetWorksToAllot(long userId, long periodId,long allotTimes)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetWorksIDByPeriodIDAndUserID", userId, periodId, allotTimes);
            return ds;
        }
        /// <summary>
        /// 获取学生待评审的作品
        /// </summary>
        /// <param name="expertID">评分人ID</param>
        /// <returns></returns>
        public static DataSet GetWorksToEvaluate(long expertID, long periodID,int isSample)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetWorksToEvaluate", expertID, periodID,isSample );
            return ds;

        }
        /// <summary>
        /// 获取评分标准的子级
        /// </summary>
        /// <returns></returns>
        public static DataSet GetScoreStandardSubLevel()
        {
            DataSet ds =DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetScoreStandardSubLevel");
            return ds;

        }
        /// <summary>
        /// 获取一个期次中所有作品以及作品对应的独创作者或者队长
        /// </summary>
        /// <returns></returns>
        public static DataSet GetUserIdWorksIdByPeriodId(long periodId)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetUserIdWorksIdByPeriodId", periodId);
            return ds;
        }
        /// <summary>
        /// 获取每个期次下分配给我评分的作品
        /// </summary>
        /// <param name="periodId">期次ID</param>
        /// <param name="expertId">我的ID</param>
        /// <returns></returns>
        public static DataSet GetWorksForMeByPeriodId(long periodId, long expertId)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetWorksForMeByPeriodID", periodId, expertId);
            return ds;
        }
        /// <summary>
        /// 通过ID获取评分明细
        /// </summary>
        /// <param name="worksExpertsID"></param>
        /// <returns></returns>
        public static DataSet GetRatingDetailsByWorksExpertID(long worksExpertsID)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetRatingDetailsByWorksExpertID", worksExpertsID);
            return ds;
        }
        /// <summary>
        /// 获取作品分组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataSet GetWorksExpertByID(long id)
        {
            DataSet ds =DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetWorksExpertByID", id);
            return ds;

        }
        /// <summary>
        /// 根据作品ID获取作品评分
        /// </summary>
        /// <param name="worksID">作品ID</param>
        /// <returns></returns>
        public static DataSet GetWorksCommentsByWorksID(long worksID)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetWorksCommentsByWorksID", worksID);
            return ds;

        }
        /// <summary>
        /// 获取提交的作品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DataSet GetWorksSubmitByID(long id)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetWorksSubmitByID", id);
            return ds;
        }
        /// <summary>
        /// 通过用户账户获取分配给我待评价的作品
        /// </summary>
        /// <param name="userAccount">用户账户</param>
        /// <returns></returns>
        public static DataSet GetWorksForMeToScore(string userAccount)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetWorksForMeToScore", userAccount);
            return ds;
        }
        /// <summary>
        /// 通过作品类别获取评分标准
        /// </summary>
        /// <param name="worksTypeID"></param>
        /// <returns></returns>
        public static DataSet GetScoreStandardByWorksType(int worksTypeID,long periodID)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetScoreStandardByWorksType", worksTypeID,periodID);
            return ds;
        }
        /// <summary>
        /// 根据作品ID和作品类别获取作品文件
        /// </summary>
        /// <param name="WorksID">作品ID</param>
        /// <param name="Type">作品类别</param>
        /// <returns></returns>
        public static DataSet GetWorksFile(long WorksID, int Type)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetWorksFile", WorksID, Type);
            return ds;
        }
        public static DataSet GetWorksType()
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetWorksType");
            return ds;
        }
        public static long UpdateWorksSubmit(SqlTransaction trans, DataRow dr)
        {
            return (DAL.SqlHelper.ExecuteAppointedParameters(trans, "UpdateWorksSubmit", dr));
        }
        public static long UpdateWorksExpert(SqlTransaction trans, DataRow dr)
        {
            return (DAL.SqlHelper.ExecuteAppointedParameters(trans, "UpdateWorksExpert", dr));
        }
        /// <summary>
        /// 作品评分明细
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static long InsertRatingDetails(SqlTransaction trans, DataRow dr)
        {
            return ((long)DAL.SqlHelper.ExecuteNonQueryTypedParamsOutput(trans, "InsertRatingDetails", dr)[0].Value);
        }
        public static long UpdateRatingDetails(SqlTransaction trans, DataRow dr)
        {
            return (DAL.SqlHelper.ExecuteAppointedParameters(trans, "UpdateRatingDetails", dr));
        }
        /// <summary>
        /// 加入新记录
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static long InsertWorksComments(DataRow dr)
        {
            return ((long)DAL.SqlHelper.ExecuteNonQueryTypedParamsOutput(DAL.DataProvider.ConnectionString, "InsertWorksExpert", dr)[0].Value);
        }

        #region 查询
       
        public static DataSet GetWorksTypeForEnroll()
        {
            DataSet ds = SqlHelper.ExecuteDataset(DataProvider.ConnectionString, "GetWorksTypeForEnroll");
            return ds;
        }

        public static DataSet GetWorksByWorksID(long WorksID)
        {
            DataSet ds = SqlHelper.ExecuteDataset(DataProvider.ConnectionString, "GetWorksByWorksID", WorksID);
            return ds;
        }
        public static DataSet GetWorksForSelf(string Account)
        {
            DataSet ds = SqlHelper.ExecuteDataset(DataProvider.ConnectionString, "GetWorksForSelf", Account);
            return ds;
        }
        
        public static DataSet GetContestTime(long ContestID)
        {
            DataSet ds = SqlHelper.ExecuteDataset(DataProvider.ConnectionString, "GetContestTime", ContestID);
            return ds;
        }
        /// <summary>
        /// 获取我的作品
        /// </summary>
        /// <param name="userAccount"></param>
        /// <returns></returns>
        public static DataSet GetMyWorks(string userAccount)
        {
            DataSet ds = SqlHelper.ExecuteDataset(DataProvider.ConnectionString, "GetMyWorks", userAccount);
            return ds;
        }
        /// <summary>
        /// 获取公示的作品，以提交 作品并审批通过
        /// </summary>
        /// <returns></returns>
        public static DataSet GetWorksPublicByWorksId(long periodId, long worksId)
        {
            DataSet ds = SqlHelper.ExecuteDataset(DataProvider.ConnectionString, "GetWorksPublicByWorksID", periodId, worksId);
            return ds;
        }
        #endregion
        #region 插入

       
        public static long InsertWorks(DataRow dr)
        {
            return ((long)SqlHelper.ExecuteNonQueryTypedParamsOutput(DataProvider.ConnectionString, "InsertWorks", dr)[0].Value);
        }
        public static long InsertWorksImages(DataRow dr)
        {
            return ((long)SqlHelper.ExecuteNonQueryTypedParamsOutput(DataProvider.ConnectionString, "InsertWorksImages", dr)[0].Value);
        }
        #endregion
        #region 更新
        public static long UpdateWorksFile(DataRow dr)
        {
            return (SqlHelper.ExecuteAppointedParameters(DataProvider.ConnectionString, "UpdateWorksFile", dr));
        }
        public static long UpdateWorksFileForSize(DataRow dr)
        {
            return (SqlHelper.ExecuteAppointedParameters(DataProvider.ConnectionString, "UpdateWorksFileForSize", dr));
        }
        

        public static long UpdateWorksInfo(DataRow dr)
        {
            return (SqlHelper.ExecuteAppointedParameters(DataProvider.ConnectionString, "UpdateWorksInfo", dr));
        }
        //
        public static long UpdateWorksCode(DataRow dr)
        {
            return (SqlHelper.ExecuteAppointedParameters(DataProvider.ConnectionString, "UpdateWorksCode", dr));
        }

        /// <summary>
        /// 对一个作品每分配评分一次,作品分配次数+1
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static long UpdateWorksAllotTimes(DataRow dr)
        {
            return (SqlHelper.ExecuteAppointedParameters(DataProvider.ConnectionString, "UpdateWorksAllotTimes", dr));
        }

        /// <summary>
        /// 获取每个作品的评分信息
        /// </summary>
        /// <param name="worksID"></param>
        /// <returns></returns>
        public static DataSet GetWorksNumByPeriodID(long PeriodID)
        {
            DataSet ds = DAL.SqlHelper.ExecuteDataset(DAL.DataProvider.ConnectionString, "GetWorksNumByPeriodID", PeriodID);
            return ds;
        }

        #endregion
     

    }
}
