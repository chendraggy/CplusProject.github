using WITS.Framework.Data.DBData;
using WITS.Framework.Interface;
using WITS.Framework.Parameters;
using WITS.Framework.Resource;
using WITS.Framework.Returns;
using WITS.Framework.WebBuilder.Public;
using WITS.Framework.WebBuilder.WebLogic;
using WITS.Framework.WebBuilder.WebParameters;

namespace BMP.Noevir.Net.AppCode.Logic.Public
{
    public class AjaxCustomLogic : WebBaseLogic, IQuery
    {
        public QueryReturn Query(ParametersStd p)
        {
            WebManageParameter wmp = (WebManageParameter)p;
            wmp.IsOperatorLog = false;
            QueryReturn rtn = new QueryReturn();
            DBAP dbap = ADBAccess.GetDBParametersDefine();
            if (wmp.GetValue<string>("QueryType") == "CheckNOExists")
                dbap.SQL = @"select * from NV_CHECK_DATA where CKDT_CHK_NO=@QueryID and  CKDT_STS NOT IN ('3','4','5')";
            if (wmp.GetValue<string>("QueryType") == "AgentAndEmp")
                dbap.SQL = @"SELECT CUST_NAME FROM dbo.NV_CUSTOM WHERE (CUST_TYPE='E' or CUST_TYPE='F') AND  CUST_ID = @QueryID and (@QueryID1='1' or (@QueryID1='0' and CUST_FRAN_STS <> 'Z'))";
            if (wmp.GetValue<string>("QueryType") == "Employee")
                dbap.SQL = @"SELECT CUST_NAME FROM dbo.NV_CUSTOM WHERE CUST_TYPE='E' AND  CUST_ID = @QueryID";

            if (wmp.GetValue<string>("QueryType") == "Agent")
                //Moeify for NVRU-135 by xuxiaoxang 20130910 begin
                //訂單資料登錄，代理店須可以選到員工
                dbap.SQL = @"SELECT CUST_NAME FROM dbo.NV_CUSTOM WHERE   CUST_ID = @QueryID";
            //Moeify for NVRU-135 by xuxiaoxang 20130910 End

            if (wmp.GetValue<string>("QueryType") == "AgentNew")
            {
                //Modify for NVRU-135  by xuxiaoxang 20130910 begin
                //訂單資料登錄，代理店須可以選到員工
                dbap.SQL = @"SELECT CUST_NAME FROM dbo.NV_CUSTOM WHERE  cust_fran_sts ='A' AND  CUST_ID = @QueryID";
                //Modify for NVRU-135 by xuxiaoxang 20130910 end
            }

            if (wmp.GetValue<string>("QueryType") == "AgentCodeForDelivery")
                dbap.SQL = @"select CUST_ID,CUST_NAME,CUST_BLAK,CUST_REMARK,CUST_SHIP_ZIP,CUST_SHIP_TEL,
                        isnull(CUST_SHIP_ZIP,'')+'|'+isnull(CUST_SHIP_ADDR,'')+'|'+isnull(CUST_SHIP_TEL,'')+','+isnull(CUST_SHIP_ZIP2,'')+'|'+isnull(CUST_SHIP_ADDR2,'')+'|'+isnull(CUST_SHIP_TEL2,'')+','+isnull(CUST_SHIP_ZIP3,'')+'|'+isnull(CUST_SHIP_ADDR3,'')+'|'+isnull(CUST_SHIP_TEL3,'') as addrzip,
                        replace(convert(varchar,convert(money,isnull(CUST_FRAN_CON_CREDIT,0)),1),'.00','') as out_credit, 
                        replace(convert(varchar,convert(money,isnull(V1.F1,0)),1),'.00','') as used_credit,
                        EMP_NAME AS CUST_FRONT_NAME 
                        from NV_CUSTOM LEFT JOIN (SELECT CSST_CUST_ID,SUM(CSST_PRICE*CSST_QTY) F1 FROM NV_CUST_STOCK WHERE  CSST_SAL_ATTR in ('A') GROUP BY CSST_CUST_ID) V1 
                        ON NV_CUSTOM.CUST_ID=V1.CSST_CUST_ID 
                        LEFT join OR_EMP on EMP_CODE=CUST_FRONT
                        WHERE (@QueryID2='1' or (@QueryID2='0' and cust_fran_sts='A')) 
                        and CUST_ID = @QueryID0

                        SELECT count(*) FROM NV_ACCT_DATA_DTL 
                        where ADDT_ACDT_END>(select Field_Value from NV_CODE_TABLE where TYPE_NO='000' and Field_No='001') 
                        and ADDT_ACDT_YM<@QueryID1 AND ADDT_ACDT_CUST_ID=@QueryID0 AND ADDT_YM=(SELECT SPAR_ACCT_YM FROM NV_SYS_PARA) ";

            if (wmp.GetValue<string>("QueryType") == "Bank")
                dbap.SQL = @"SELECT BANK_NAME FROM nv_bank WHERE BANK_CODE=@QueryID";

            if (wmp.GetValue<string>("QueryType") == "QueryProdGiftCode1")
                dbap.SQL = @"select PDML_NAME AS NAME from NV_PROD_MAIN_LINE  WHERE PDML_NO=@QueryID";

            if (wmp.GetValue<string>("QueryType") == "QueryProdGiftCode2")
                dbap.SQL = @"SELECT PDSL_NAME AS NAME FROM NV_PROD_SUB_LINE  WHERE (PDSL_MNO+PDSL_SNO)=@QueryID";

            if (wmp.GetValue<string>("QueryType") == "QueryProdGiftCode3")
                dbap.SQL = @"SELECT PROD_CHN_NAME AS NAME FROM dbo.NV_PRODUCT  WHERE PROD_ID=@QueryID";
            if (wmp.GetValue<string>("QueryType") == "QueryProdGiftCodeNew")
            {
                string QueryID = wmp.GetValue<string>("QueryID");
                string ShipmentDate = wmp.GetValue<string>("ShipmentDate").Substring(0, 6);
                dbap.SQL = @"SELECT distinct PROD_CHN_NAME AS NAME FROM dbo.NV_PRODUCT  
                            inner join NV_PROD_ATTR on NV_PROD_ATTR.PDAT_PROD_ID=NV_PRODUCT.PROD_ID  
                            inner join NV_PROD_PRICE on NV_PROD_PRICE.PDPC_PROD_ID=NV_PRODUCT.PROD_ID
                            WHERE PROD_ID='" + QueryID + "'and PROD_SALE_DATE_E>='" + ShipmentDate + "'+'01' and PROD_SALE_DATE_S<='" + ShipmentDate + "'+'30' and NV_PROD_ATTR.PDAT_MTH_S<='" + ShipmentDate + "' and NV_PROD_ATTR.PDAT_MTH_E>='" + ShipmentDate + "' and NV_PROD_PRICE.PDPC_MTH_S<='" + ShipmentDate + "' and NV_PROD_PRICE.PDPC_MTH_E>='" + ShipmentDate + "'";
            }
            if (wmp.GetValue<string>("QueryType") == "I")
            {
                if (wmp.Module.GetValue<string>("QueryID") != "")
                {
                    dbap.SQL += " and prod_ID= @QueryID";
                    dbap.SetValue("QueryID", wmp.Module.GetValue<string>("QueryID"));
                }
                dbap.SQL = @"SELECT prod_ID AS PRONO,PROD_CHN_NAME AS PRONAME,1 AS PROGOOD_QTY,CONVERT(VARCHAR,CAST(PROD_SRP_NT AS MONEY),1) AS PROSRP_NT from NV_PRODUCT WHERE 1=1 " + dbap.SQL;
            }

            if (wmp.GetValue<string>("QueryType") == "O")
            {
                if (wmp.Module.GetValue<string>("STCK_CODE") != "")
                {
                    dbap.SQL += " and PDST_STCK_CODE= @STCK_CODE";
                    dbap.SetValue("STCK_CODE", wmp.Module.GetValue<string>("STCK_CODE"));
                }
                if (wmp.Module.GetValue<string>("QueryID") != "")
                {
                    dbap.SQL += " and PDST_PROD_ID= @QueryID";
                    dbap.SetValue("QueryID", wmp.Module.GetValue<string>("QueryID"));
                }
                dbap.SQL = @"SELECT DISTINCT PDST_PROD_ID AS PRONO,PROD_CHN_NAME AS PRONAME,PDST_GOOD_QTY AS PROGOOD_QTY,CONVERT(VARCHAR,CAST(PROD_SRP_NT AS MONEY),1) AS PROSRP_NT 
                    FROM NV_PROD_STOCK 
                    RIGHT JOIN NV_PRODUCT ON PROD_ID=PDST_PROD_ID 
                    WHERE 1=1 " + dbap.SQL;
            }

            if (wmp.GetValue<string>("QueryType") == "QueryGroupProduct")
                dbap.SQL = @"SELECT PROD_CHN_NAME  FROM NV_PRODUCT where PROD_ID=@QueryID";

            if (wmp.GetValue<string>("QueryType") == "QueryProdPrice")
                dbap.SQL = @"SELECT PROD_CHN_NAME+';'+replace(convert(varchar,convert(money,PROD_SRP_NT),1),'.00','')  AS PRICE FROM dbo.NV_PRODUCT  WHERE PROD_ID=@QueryID";

            if (wmp.GetValue<string>("QueryType") == "QueryStockProPrice")
                dbap.SQL = @"SELECT PROD_CHN_NAME+';'+replace(convert(varchar,convert(money,CSST_PRICE),1),'.00','')+';'+
                            CSST_SAL_ATTR +';'+(CASE WHEN PDAT_TYPE_CODE='1' THEN '現銷' WHEN PDAT_TYPE_CODE='2' THEN '委託' END)
                            FROM (NV_CUST_STOCK RIGHT JOIN NV_PRODUCT ON PROD_ID=CSST_PROD_ID)  
                            RIGHT JOIN NV_PROD_ATTR ON PDAT_PROD_ID=CSST_PROD_ID AND 
                            CSST_ATTR_YM >= PDAT_MTH_S AND CSST_ATTR_YM <= PDAT_MTH_E  
                            WHERE CSST_CUST_ID=@QueryID2 and CSST_PROD_ID=@QueryID1
                            GROUP BY CSST_PROD_ID,PROD_CHN_NAME,CSST_PRICE,CSST_SAL_ATTR,PDAT_TYPE_CODE,
                            PDAT_FEAT_FLAG,PDAT_ACC_ATTR,PDAT_DISCOUNT_FLAG  
                            Having Sum(CSST_QTY) > 0";
            if (wmp.GetValue<string>("QueryType") == "QueryProductCodeForDelivery")
                dbap.SQL = @"--declare @AccYM varchar(6)
                            --select @AccYM=(SELECT SPAR_ACCT_YM FROM NV_SYS_PARA)
                            declare @result varchar(max)  
                            select @result = ''  
                            select @result = @result+convert(varchar(18),PDPC_PRICE) + '|'  
                            from (SELECT PDPC_MTH_E,PDPC_PRICE  FROM NV_PROD_PRICE  
                            WHERE PDPC_PROD_ID=@QueryID0 AND  PDPC_MTH_S <=@AccYM AND PDPC_MTH_E>=@AccYM) T9
                            order by T9.PDPC_MTH_E desc
                            select @result=@result+'0'

                        select PROD_ID as prodid,PROD_CHN_NAME as prodname, T1.Field_Content as Type,cust_qty as custQty ,prod_qty as prodQty,PROD_ATTR as attr, price, @result as pricelist
                        from NV_PRODUCT 
                        inner join NV_PROD_ATTR ON NV_PRODUCT.PROD_ID=NV_PROD_ATTR.PDAT_PROD_ID AND NV_PROD_ATTR.PDAT_MTH_S<=@AccYM AND NV_PROD_ATTR.PDAT_MTH_E>=@AccYM
                        left join (SELECT CSST_PROD_ID,SUM(CSST_QTY) cust_qty FROM NV_CUST_STOCK WHERE CSST_CUST_ID=@QueryID1 GROUP BY CSST_PROD_ID) AS V1 ON NV_PRODUCT.PROD_ID=CSST_PROD_ID
                        left join (SELECT PDST_PROD_ID,SUM(PDST_GOOD_QTY) prod_qty FROM NV_PROD_STOCK WHERE PDST_STCK_CODE=@QueryID2 GROUP BY PDST_PROD_ID) AS V2 ON NV_PRODUCT.PROD_ID=PDST_PROD_ID
                        left join NV_CODE_TABLE T1 on PDAT_TYPE_CODE=T1.Field_No and T1.Type_No='001'
                        left join (select PDPC_PROD_ID,max(PDPC_PRICE) as price FROM NV_PROD_PRICE WHERE PDPC_MTH_S <=@AccYM AND PDPC_MTH_E>=@AccYM group by PDPC_PROD_ID ) T7 on PDPC_PROD_ID=PROD_ID 
                        WHERE PROD_SALE_DATE_S<=convert(varchar(8),getDate(),112) AND PROD_SALE_DATE_E>=convert(varchar(8),getDate(),112) and PROD_ID=@QueryID0 ";
            if (wmp.GetValue<string>("QueryType") == "QueryProductCodeForDelivery1")
                dbap.SQL = @" declare @AccYM varchar(6)
                            select @AccYM=(SELECT SPAR_ACCT_YM FROM NV_SYS_PARA)
                            declare @result varchar(max)  
                            select @result = ''  
                            select @result = @result+convert(varchar(18),PDPC_PRICE) + '|'  
                            from (SELECT PDPC_MTH_E,PDPC_PRICE  FROM NV_PROD_PRICE  
                            WHERE PDPC_PROD_ID=@QueryID0 AND  PDPC_MTH_S <=@AccYM AND PDPC_MTH_E>=@AccYM) T9
                            order by T9.PDPC_MTH_E desc
                            select @result=@result+'0'

                        select PROD_ID as prodid,PROD_CHN_NAME as prodname, T1.Field_Content as Type,cust_qty as custQty ,prod_qty as prodQty,PROD_ATTR as attr, price, @result as pricelist
                        from NV_PRODUCT 
                        inner join NV_PROD_ATTR ON NV_PRODUCT.PROD_ID=NV_PROD_ATTR.PDAT_PROD_ID AND NV_PROD_ATTR.PDAT_MTH_S<=@AccYM AND NV_PROD_ATTR.PDAT_MTH_E>=@AccYM
                        left join (SELECT CSST_PROD_ID,SUM(CSST_QTY) cust_qty FROM NV_CUST_STOCK WHERE CSST_CUST_ID=@QueryID1 GROUP BY CSST_PROD_ID) AS V1 ON NV_PRODUCT.PROD_ID=CSST_PROD_ID
                        left join (SELECT PDST_PROD_ID,SUM(PDST_GOOD_QTY) prod_qty FROM NV_PROD_STOCK WHERE PDST_STCK_CODE=@QueryID2 GROUP BY PDST_PROD_ID) AS V2 ON NV_PRODUCT.PROD_ID=PDST_PROD_ID
                        left join NV_CODE_TABLE T1 on PDAT_TYPE_CODE=T1.Field_No and T1.Type_No='001'
                        left join (select PDPC_PROD_ID,max(PDPC_PRICE) as price FROM NV_PROD_PRICE WHERE PDPC_MTH_S <=@AccYM AND PDPC_MTH_E>=@AccYM group by PDPC_PROD_ID ) T7 on PDPC_PROD_ID=PROD_ID 
                        WHERE PROD_SALE_DATE_S<=convert(varchar(8),getDate(),112) AND PROD_SALE_DATE_E>=convert(varchar(8),getDate(),112) and PROD_ID=@QueryID0 ";

            dbap.SQL_Parameters.Add(wmp.Module);
            Dao.Open(WebConfigString.DBConn);
            DataSetStd dts = Dao.Query(dbap);
            Dao.Close();
            rtn.R_DataSet = dts;
            return rtn;
        }

    }
}