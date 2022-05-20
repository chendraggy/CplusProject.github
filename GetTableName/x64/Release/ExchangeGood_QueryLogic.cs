/******************************************************************
 *  作　　者：ZhangLei
 *  功能說明：商品退貨登錄
 *  創建日期：2012/11/15
 *******************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WITS.Framework.WebBuilder.WebLogic;
using WITS.Framework.Interface;
using WITS.Framework.WebBuilder.WebParameters;
using WITS.Framework.Returns;
using WITS.Framework.Parameters;
using WITS.Framework.Resource;
using WITS.Framework.WebBuilder.Public;
using WITS.Framework.Data.DBData;
using System.Data;
using WITS.Framework.Common;

namespace BMP.Noevir.Net.AppCode.Logic.STOCK
{
    public class ExchangeGood_QueryLogic : WebBaseLogic, IQueryByPage, IQuery, IUpdate, IInsert
    {
        public string QueryByPageSQL_End(ParametersStd p, ref DBOParameterCollection dbp)
        {
            return string.Empty;
        }

        public string QueryByPageSQL_Prepare(ParametersStd p, ref DBOParameterCollection dbp)
        {
            return string.Empty;
        }

        public string QueryByPageSQL_QueryByPage(ParametersStd p, ref DBOParameterCollection dbp)
        {
            WebManageParameter wmp = (WebManageParameter)p;
            string actionType = p.GetValue<string>("action");
            string sql = string.Empty;
            if (actionType.Equals("QueryWIN"))
            {
                if (wmp.Module.GetValue<string>("BAKM_NO").ToString() != "")
                {
                    sql += " AND a.BAKM_NO LIKE  '%" + wmp.Module.GetValue<string>("BAKM_NO") + "%'";
                }

                if (wmp.Module.GetValue<string>("UserID").ToString() != "")
                {
                    sql += " AND a.ADD_USER_ID LIKE  '%" + wmp.Module.GetValue<string>("UserID") + "%'";
                }

                if (wmp.Module.GetValue<string>("BAKM_STOCK").ToString() != "")
                {
                    sql += " AND a.BAKM_STOCK=@BAKM_STOCK";
                    dbp.SetValue("BAKM_STOCK", wmp.Module.GetValue<string>("BAKM_STOCK"));
                }

                if (wmp.Module.GetValue<string>("BAKM_DATES_X").ToString() != "")
                {
                    sql += " AND a.BAKM_DATE >='" + (wmp.Module.GetValue<string>("BAKM_DATES_X").Replace("/", "")) + "'";
                }
                if (wmp.Module.GetValue<string>("BAKM_DATEE_X").ToString() != "")
                {
                    sql += " AND a.BAKM_DATE <='" + (wmp.Module.GetValue<string>("BAKM_DATEE_X").Replace("/", "")) + "'";
                }

                if (wmp.Module.GetValue<string>("BAKM_ACCT_YM").ToString() != "")
                {
                    sql += " AND a.BAKM_ACCT_YM =replace('" + wmp.Module.GetValue<string>("BAKM_ACCT_YM") + "','/','')";
                }
                if (wmp.Module.GetValue<string>("BAKM_APPLY_CODE").ToString() != "")
                {
                    sql += " AND a.BAKM_APPLY_CODE =@BAKM_APPLY_CODE";
                    dbp.SetValue("BAKM_APPLY_CODE", wmp.Module.GetValue<string>("BAKM_APPLY_CODE"));
                }
                if (wmp.Module.GetValue<string>("BAKM_CUST").ToString() != "")
                {
                    sql += " AND a.BAKM_CUST= @BAKM_CUST";
                    dbp.SetValue("BAKM_CUST", wmp.Module.GetValue<string>("BAKM_CUST"));
                }
                if (wmp.Module.GetValue<string>("Field_No").ToString() != "")
                {
                    sql += " AND a.BAKM_STS = @BAKM_STS";
                    dbp.SetValue("BAKM_STS", wmp.Module.GetValue<string>("Field_No"));
                }
                return @" SELECT a.BAKM_NO,
                         (BAKM_CONFIRM+BAKM_CANCEL+BAKM_STS)'Check',
                         d.Field_Content,
                         SUBSTRING(a.BAKM_DATE,1,4)+'/'+SUBSTRING(a.BAKM_DATE,5,2)+'/'+SUBSTRING(a.BAKM_DATE,7,2)BAKM_DATE,
                         dbo.GetFormatString(test.price,0)price,
                         c.STCK_NAME,
                        a.BAKM_CUST, 
                        b.CUST_NAME,a.ADD_USER_ID,
                        a.BAKM_STS,a.BAKM_CONFIRM,a.BAKM_CANCEL
                        FROM  NV_BACK_MST a
                        LEFT JOIN NV_CUSTOM b ON a.BAKM_CUST=b.CUST_ID
                        LEFT JOIN NV_STOCK c ON c.STCK_CODE=a.BAKM_STOCK
                        LEFT JOIN (select Field_No ,Field_Content from NV_CODE_TABLE where Type_No='026')
                        d ON a.BAKM_STS=d.Field_No
                        LEFT JOIN (select BAKD_NO, SUM(NV_BACK_DTL.BAKD_PRICE*NV_BACK_DTL.BAKD_QTY) price 
                        FROM NV_BACK_DTL 
                        GROUP by NV_BACK_DTL.BAKD_NO)test 
                        ON test.BAKD_NO=a.BAKM_NO 
                        WHERE 1=1 
                        AND a.BAKM_NO NOT LIKE '%RV%' " + sql;
            }

            if (actionType.Equals("QueryByLoad"))
            {
                if (wmp.Module.GetValue<string>("BAKM_CUST").ToString() != "")
                {
                    sql += " and CSST_CUST_ID like '%" + wmp.Module.GetValue<string>("BAKM_CUST") + "%'";
                }
                if (wmp.Module.GetValue<string>("BAKM_ACCT_YM").ToString() != "")
                {
                    sql += " AND " + wmp.Module.GetValue<string>("BAKM_ACCT_YM").Replace("/", "") + " >= PDAT_MTH_S AND " + wmp.Module.GetValue<string>("BAKM_ACCT_YM").Replace("/", "") + " <= PDAT_MTH_E ";
                }
                if (wmp.Module.GetValue<string>("PROD_CHN_NAME").ToString() != "")
                {
                    sql += " and PROD_CHN_NAME like '%" + wmp.Module.GetValue<string>("PROD_CHN_NAME") + "%'";
                }
                if (wmp.Module.GetValue<string>("PROD_ID").ToString() != "")
                {
                    sql += " and CSST_PROD_ID like '%" + wmp.Module.GetValue<string>("PROD_ID") + "%'";
                }
            }
            return @"   SELECT CSST_PROD_ID,
                        PROD_CHN_NAME ,
                        (SELECT  FIELD_CONTENT  FROM NV_CODE_TABLE WHERE Type_No ='004' AND Field_No =CSST_SAL_ATTR) 'CSST_SAL_ATTR',
                        CSST_SAL_ATTR as attrCode,
                        SUM(CSST_QTY) CSST_QTY 
                        FROM (NV_CUST_STOCK inner JOIN NV_PRODUCT ON PROD_ID=CSST_PROD_ID 
                        AND CSST_ATTR_YM<='" + wmp.Module.GetValue<string>("BAKM_ACCT_YM").Replace("/", "") + "')" +
                        @" RIGHT JOIN NV_PROD_ATTR ON PDAT_PROD_ID=CSST_PROD_ID where 1=1 " + sql +
                        @" GROUP BY CSST_PROD_ID,PROD_CHN_NAME,CSST_SAL_ATTR,CSST_QTY Having Sum(CSST_QTY) > 0 ";
        }

        public QueryReturn Query(WITS.Framework.Parameters.ParametersStd p)
        {
            WebManageParameter wmp = (WebManageParameter)p;
            QueryReturn rtn = new QueryReturn();
            DBAP dbap = ADBAccess.GetDBParametersDefine();
            if (p.GetValue<string>("QueryType") == "getSTS")
            {
                dbap.SQL = @" SELECT BAKM_STS 
                              FROM NV_BACK_MST 
                              WHERE BAKM_NO=@BAKM_NO ";
            }
            if (p.GetValue<string>("QueryType") == "NewID")
            {
                dbap.SQL = @" SELECT case when max(BAKM_NO) IS NULL then  'R'+substring(convert(varchar(8),getDate(),112),3,6)+'001' 
                              ELSE 'R'+substring(convert(varchar(8),getDate(),112),3,6)+right('000'+convert(varchar(4),convert(int,right(max(BAKM_NO),3))+1),3) end 
                              FROM NV_BACK_MST
                              WHERE BAKM_NO LIKE 'R'+substring(convert(varchar(8),getDate(),112),3,6)+'%'";
            }
            if (p.GetValue<string>("QueryType") == "DrpStock")
            {
                dbap.SQL = @"select distinct NV_STOCK.STCK_NAME,NV_STOCK.STCK_CODE 
                            from NV_STOCK 
                            order by STCK_CODE";
            }
            if (p.GetValue<string>("QueryType") == "DrpStatus")
            {
                dbap.SQL = @" select Field_No ,Field_Content 
                              from NV_CODE_TABLE where Type_No='026' 
                              order by Field_No";
            }
            if (p.GetValue<string>("QueryType") == "Judement")
            {
                dbap.SQL = @"SELECT BAKM_NO,BAKM_CANCEL,BAKM_STS FROM NV_BACK_MST 
                                inner join (select * from dbo.f_SplitToNvarchar(@DelBAKM_NO,',')) a
                                on a.a=NV_BACK_MST.BAKM_NO
                                where  NV_BACK_MST.BAKM_CANCEL ='Y' or NV_BACK_MST.BAKM_STS=9";
            }
            if (p.GetValue<string>("QueryType") == "drpOut")
            {
                dbap.SQL = @"select Field_Content STCK_NAME,Field_No STCK_CODE from NV_CODE_TABLE where Type_No='031'";
            }
            if (p.GetValue<string>("QueryType") == "txtTime")
            {
                dbap.SQL = @"SELECT SUBSTRING(SPAR_ACCT_YM,1,4)+'/'+SUBSTRING(SPAR_ACCT_YM,5,2)SPAR_ACCT_YM FROM NV_SYS_PARA";
            }
            if (p.GetValue<string>("QueryType") == "GridViewInert")
            {
                string[] str = p.GetValue<string>("InertBAKD_PROD").Split(',');
                string[] strAttr = p.GetValue<string>("InertBAKD_PROD_ATTR").Split(',');
                string strCon = "";
                for (int i = 0; i < str.Length; i++)
                {
                    if (p.GetValue<string>("InertBAKD_PROD_ATTR") == "")
                        strCon += "(CSST_PROD_ID='" + str[i] + "') or";
                    else
                        strCon += "(CSST_PROD_ID='" + str[i] + "' and CSST_SAL_ATTR='" + strAttr[i] + "') or";
                }

                if (strCon != "")
                {
                    strCon = strCon.Substring(0, strCon.Length - 2);
                    strCon = " and (" + strCon + ")";
                }

                if (str.Length >= 2)
                {
                    #region GrigView
                    dbap.SQL = @"SELECT CSST_PROD_ID,
                                PROD_CHN_NAME ,
                                1 as CSST_QTY,  
                                dbo.GetFormatString(CSST_PRICE,0) as CSST_PRICE,
                                dbo.GetFormatString(CSST_PRICE,0) as 'AlwaysPrice',
                                (SELECT  FIELD_CONTENT  FROM NV_CODE_TABLE WHERE Type_No ='004' AND Field_No =CSST_SAL_ATTR) 'CSST_SAL_ATTR',
                                (SELECT  FIELD_CONTENT  FROM NV_CODE_TABLE WHERE Type_No ='001' AND Field_No =PDAT_TYPE_CODE) TypeName,  
                                PDAT_FEAT_FLAG,
                                PDAT_ACC_ATTR,
                                PDAT_DISCOUNT_FLAG,
                                '' as BAKD_DESC
                                FROM (NV_CUST_STOCK inner JOIN NV_PRODUCT ON PROD_ID=CSST_PROD_ID and CSST_ATTR_YM<='" + p.GetValue<string>("BAKD_ATTR_YM") + "')" +
                                @" RIGHT JOIN NV_PROD_ATTR ON PDAT_PROD_ID=CSST_PROD_ID
                                where 1=1  and   '" + p.GetValue<string>("BAKD_ATTR_YM") + "'" +
                                @" >=PDAT_MTH_S and PDAT_MTH_E>='" + p.GetValue<string>("BAKD_ATTR_YM") + "'" +
                        //@" and  CSST_PROD_ID in (select * from dbo.f_SplitToNvarchar('" + p.GetValue<string>("InertBAKD_PROD") +
                                strCon +
                                @" and CSST_CUST_ID='" + p.GetValue<string>("InertBAKM_CUST") + "'" +
                                @" GROUP BY CSST_PROD_ID,PROD_CHN_NAME,
                                CSST_PRICE,CSST_SAL_ATTR,PDAT_TYPE_CODE,
                                PDAT_FEAT_FLAG,PDAT_ACC_ATTR,
                                PDAT_DISCOUNT_FLAG,
                                CSST_PRICE  
                                Having Sum(CSST_QTY) > 0";
                    #endregion
                }
                else
                {
                    //20151208 JamesDing Modify Start Add CSST_QTY in Group
                    #region GrigView
                    dbap.SQL = @"SELECT CSST_PROD_ID,
                                PROD_CHN_NAME ,
                                SUM(CSST_QTY) CSST_QTY,  
                                dbo.GetFormatString(CSST_PRICE,0) as CSST_PRICE,
                                dbo.GetFormatString(SUM(CSST_QTY*CSST_PRICE),0) as 'AlwaysPrice',
                                (SELECT  FIELD_CONTENT  FROM NV_CODE_TABLE WHERE Type_No ='004' AND Field_No =CSST_SAL_ATTR) 'CSST_SAL_ATTR',
                                (SELECT  FIELD_CONTENT  FROM NV_CODE_TABLE WHERE Type_No ='001' AND Field_No =PDAT_TYPE_CODE) TypeName,  
                                PDAT_FEAT_FLAG,
                                PDAT_ACC_ATTR,
                                PDAT_DISCOUNT_FLAG,  
                                '' as BAKD_DESC
                                FROM (NV_CUST_STOCK RIGHT JOIN NV_PRODUCT ON PROD_ID=CSST_PROD_ID and CSST_ATTR_YM<='" + p.GetValue<string>("BAKD_ATTR_YM") + "')" +
                                @" RIGHT JOIN NV_PROD_ATTR ON PDAT_PROD_ID=CSST_PROD_ID
                                where 1=1  and   '" + p.GetValue<string>("BAKD_ATTR_YM") + "'" +
                                @" >=PDAT_MTH_S and PDAT_MTH_E>='" + p.GetValue<string>("BAKD_ATTR_YM") + "'" +
                        //@" and  CSST_PROD_ID in (select * from dbo.f_SplitToNvarchar('" + p.GetValue<string>("InertBAKD_PROD") +
                                strCon +
                                @" and CSST_CUST_ID='" + wmp.Module.GetValue<string>("BAKM_CUST") + "'" +
                                @" GROUP BY CSST_PROD_ID,PROD_CHN_NAME,
                                CSST_PRICE,CSST_SAL_ATTR,PDAT_TYPE_CODE,
                                PDAT_FEAT_FLAG,PDAT_ACC_ATTR,
                                PDAT_DISCOUNT_FLAG,
                                CSST_PRICE,CSST_QTY
                                Having Sum(CSST_QTY) > 0";
                    #endregion
                    //20151208 JamesDing Modify End
                }
            }
            if (p.GetValue<string>("QueryType") == "GridViewModfily")
            {
                #region Grid

                dbap.SQL = @"select 
                            BAKD_PROD 'CSST_PROD_ID',
                            PROD_CHN_NAME ,
                            sum(BAKD_QTY) 'CSST_QTY',
                            dbo.GetFormatString(BAKD_PRICE,0) 'CSST_PRICE',
                            (SELECT  FIELD_CONTENT  FROM NV_CODE_TABLE WHERE Type_No ='004' AND Field_No =BAKD_SAL_ATTR) 'CSST_SAL_ATTR',
                            dbo.GetFormatString(SUM(BAKD_QTY*BAKD_PRICE),0) as 'AlwaysPrice',
                            BAKD_DESC 'BAKD_DESC',
                            (SELECT  FIELD_CONTENT  FROM NV_CODE_TABLE WHERE Type_No ='001' AND Field_No =BAKD_TYPE) 'TypeName',
                            BAKD_FEAT_FLAG 'PDAT_FEAT_FLAG',
                            BAKD_ACC_ATTR 'PDAT_ACC_ATTR',
                            BAKD_DISCOUNT_FLAG 'PDAT_DISCOUNT_FLAG'
                            from NV_BACK_MST inner join
                            NV_BACK_DTL on BAKM_NO=BAKD_NO 
                            inner join NV_PRODUCT 
                            on NV_PRODUCT.PROD_ID=NV_BACK_DTL.BAKD_PROD
                            where NV_BACK_MST.BAKM_NO='" + p.GetValue<string>("BAKM_NOModfily") +
                            @"' group by BAKD_PROD,PROD_CHN_NAME,BAKD_PRICE,BAKD_SAL_ATTR,
                            BAKD_TYPE,BAKD_ACC_ATTR,BAKD_FEAT_FLAG,BAKD_DESC,BAKD_DISCOUNT_FLAG";
                #endregion
            }
            if (p.GetValue<string>("QueryType") == "JudementMax")
            {
                #region  JudementMax
                dbap.SQL = @"select  BAKD_NO,
                             count(
                            (case when NV_BACK_DTL.BAKD_QTY>NV_CUST_STOCK.CSST_QTY
                            then 'False' end
                            ))'reslut'
                              from NV_BACK_DTL
                             inner join NV_CUST_STOCK
                             on NV_CUST_STOCK.CSST_PROD_ID=NV_BACK_DTL.BAKD_PROD 
                              and
                             NV_BACK_DTL.BAKD_PRICE=NV_CUST_STOCK.CSST_PRICE
                            and NV_BACK_DTL.BAKD_SAL_ATTR=NV_CUST_STOCK.CSST_SAL_ATTR
                             where NV_BACK_DTL.BAKD_PROD=@BAKD_PROD and NV_BACK_DTL.BAKD_NO=@BAKD_NO
                             group by BAKD_NO";
                #endregion
            }
            if (p.GetValue<string>("QueryType") == "JudementNumber")
            {
                string[] strProdInfos = wmp.GetValue<string>("ProdInfo").Split(new char[] { '|' });
                string strCon = "";
                for (int i = 0; i < strProdInfos.Length; i++)
                {
                    if (strProdInfos[i].Trim() != "")
                    {
                        string[] strCONs = strProdInfos[i].Split(new char[] { '/' });
                        strCon += " (CSST_PROD_ID='" + strCONs[1] + "' and CSST_PRICE=" + strCONs[4].ToString().Replace(",", "") + " and Field_Content='" + strCONs[6].ToString() + "') or";
                    }

                }
                if (!string.IsNullOrEmpty(strCon))
                {
                    strCon = strCon.Substring(0, strCon.Length - 2);
                    strCon = "(" + strCon + ")";

                }
                dbap.SQL = @" select sum(CSST_QTY) as CSST_QTY,CSST_PROD_ID,CSST_PRICE,Field_Content as CSST_SAL_ATTR from NV_CUST_STOCK inner join NV_CODE_TABLE on CSST_SAL_ATTR=Field_No and type_no='004'" +
                    //                            where  CSST_PROD_ID in (select * from dbo.f_SplitToNvarchar('" + p.GetValue<string>("Finishe") + "',','))"
                    //                            + @" and CSST_PRICE in (select * from dbo.f_SplitToNvarchar('" + p.GetValue<string>("SCSST_PRICE") + "',','))" +
                            @" where 1=1 and CSST_ATTR_YM<='" + wmp.GetValue<string>("ACCYM") + "' and " + strCon +
                            @" and CSST_CUST_ID='" + wmp.Module.GetValue<string>("BAKM_CUST").ToString() + "' group by CSST_PROD_ID,CSST_PRICE,Field_Content";
            }
            if (p.GetValue<string>("QueryType") == "BAKM_CANCEL")
            {
                dbap.SQL = @"select * from NV_BACK_MST where BAKM_NO='" + p.GetValue<string>("BAKM_NOModfily") + "'";
            }
            if (p.GetValue<string>("QueryType") == "Subjoin")
            {
                dbap.SQL = @"select a.Field_Content,
                            b.PROD_CHN_NAME	 ,
                            b.PROD_SRP_NT,
                            a.Field_Content, 
                            0 number,
                            0 AllMoney
                            From nv_code_table a ,
                            Nv_product b
                            Where a.Field_Content = b.PROD_ID and 
                            a.Type_No ='024'
                            ";
            }
            if (p.GetValue<string>("QueryType") == "ModifyAddtion")
            {
                dbap.SQL = @"select BAKP_PROD Field_Content,
                            Bakp_qty number,
                            BAKP_PRICE PROD_SRP_NT,
                            (select PROD_CHN_NAME from Nv_product where PROD_ID=bakp_prod) PROD_CHN_NAME,
                            BAKP_PRICE*BAKP_QTY AllMoney from NV_BACK_PROD where BAKP_NO='" + p.GetValue<string>("BAKM_NOModfily") + "'";
            }
            if (p.GetValue<string>("QueryType") == "NV_BACK_MST")
            {
                dbap.SQL = @"select * from dbo.NV_BACK_MST where 1=0";
            }
            if (p.GetValue<string>("QueryType") == "NV_BACK_DTL")
            {
                dbap.SQL = @"select * from dbo.NV_BACK_DTL where 1=0";
            }
            if (p.GetValue<string>("QueryType") == "Type")
            {
                dbap.SQL = @"SELECT  Field_No,Field_Content  FROM NV_CODE_TABLE WHERE Type_No ='001'";
            }
            if (p.GetValue<string>("QueryType") == "QueryNV_BACK_MST")
            {
                dbap.SQL = @"select case when len(BAKM_DATE)=8 then convert(varchar(10),convert(date,BAKM_DATE),111) else BAKM_DATE end as BAKM_DATE,
                            BAKM_CONFIRM,
                            BAKM_CANCEL,
                            BAKM_STS,
                            case when len(BAKM_ACCT_YM)=6 then substring(BAKM_ACCT_YM,1,4)+'/'+substring(BAKM_ACCT_YM,5,2) else BAKM_ACCT_YM end as BAKM_ACCT_YM,
                            BAKM_STOCK,
                            BAKM_CUST,
                            NV_CUSTOM.CUST_NAME,
                            BAKM_APPLY_CODE,
                            a.CUST_NAME 'APPLYName',
                            BAKM_RETURN_CODE,
                            BAKM_DESC
                            from NV_BACK_MST
                            inner join NV_CUSTOM 
                            on CUST_ID=BAKM_CUST
                            inner join NV_CUSTOM a
                            on a.CUST_ID=BAKM_APPLY_CODE WHERE BAKM_NO ='" + p.GetValue<string>("BAKM_NOModfily") + "'";
            }
            if (p.GetValue<string>("QueryType") == "QueryATTR") //商品屬性
            {
                dbap.SQL = @"SELECT  FIELD_CONTENT,Field_No  FROM NV_CODE_TABLE WHERE Type_No ='004'";
            }
            if (p.GetValue<string>("QueryType") == "UpdateQuery")
            {
                dbap.SQL = @"select BAKD_PROD,BAKD_GOOD_OR_NG,BAKD_ATTR_YM,BAKD_SEQ from NV_BACK_DTL where BAKD_NO='" + wmp.Module.GetValue<string>("BAKM_NO") + "'";
            }
            if (p.GetValue<string>("QueryType") == "NotDeduct")
            {
                dbap.SQL = @"select Type_Name,Field_Content from NV_CODE_TABLE where Type_No='018'";
            }
            if (p.GetValue<string>("QueryType") == "Prod_Stock")
            {
                dbap.SQL = @"select * from NV_PROD_STOCK 
                                where PDST_PROD_ID=''
                                and PDST_STCK_CODE=''
                                and PDST_GOOD_QTY=''";
            }
            if (p.GetValue<string>("QueryType") == "Report")
            {
                dbap.SQL = @" SELECT ROW_NUMBER() OVER(ORDER BY BAKD_PROD) 'Number',
                                BAKD_NO,
                                BAKM_DESC,
                                BAKD_PROD,
                                STCK_NAME,
                                SUBSTRING(BAKM_DATE,1,4)+'/'+SUBSTRING(BAKM_DATE,5,2)+'/'+SUBSTRING(BAKM_DATE,7,2) 'BAKM_DATE',
                                PROD_CHN_NAME 'ProductName',
                                CUST_NAME,
                                BAKM_RETURN_CODE,
                                CUST_ID,
                                BAKD_PRICE 'ProductPrice',
                                BAKD_QTY 'ProductQuantity',
                                BAKD_PRICE*BAKD_QTY 'ProductAmout',
                                '' AS 'Deadline',
                                BAKD_DESC 'Remark'
                                FROM NV_BACK_MST
                                INNER JOIN NV_BACK_DTL
                                ON BAKM_NO=BAKD_NO
                                AND BAKM_NO NOT LIKE '%RV%'
                                INNER JOIN NV_PRODUCT 
                                ON PROD_ID=BAKD_PROD
                                INNER JOIN NV_CUSTOM
                                ON CUST_ID=BAKM_CUST
                                INNER JOIN NV_STOCK
                                ON BAKM_STOCK=NV_STOCK.STCK_CODE
                                WHERE BAKM_NO='" + p.GetValue<string>("ReportOdd") + "'";
            }
            if (p.GetValue<string>("QueryType") == "QueryReplacingGoodsReport3")
            {
                dbap.SQL_Parameters.Add("CHGM_NO", wmp.GetValue<string>("CHGM_NO"));
                dbap.SQL = @"SELECT BAKP_SEQ AS CHGD_SEQ ,BAKP_PROD AS  CHGD_PROD,dbo.NV_PRODUCT.PROD_CHN_NAME AS CHGD_PRODNAME,
                            BAKP_PRICE AS CHGD_PRICE, BAKP_QTY AS CHGD_QTY,
                            BAKP_PRICE*BAKP_QTY  AS CHGD_MONEY,'' AS StorageLife,'' AS CHGD_DESC
                            FROM NV_BACK_PROD
                            LEFT JOIN dbo.NV_PRODUCT ON prod_id=BAKP_PROD
                            WHERE BAKP_NO=@CHGM_NO and BAKP_QTY<>0";
            }
            if (p.GetValue<string>("QueryType") == "GetBAKM_CONFIRM")
            {
                dbap.SQL = @"SELECT  BAKM_CONFIRM  
                            FROM NV_BACK_MST 
                            WHERE BAKM_NO =@BAKPNO";
                wmp.Module.GetValue<string>("BAKPNo").ToString();
            }
            dbap.SQL_Parameters.Add(wmp.Module);
            Dao.Open(WebConfigString.DBConn);
            DataSetStd dts = Dao.Query(dbap);
            Dao.Close();
            rtn.R_DataSet = dts;
            return rtn;
        }

        public bool Update(ParametersStd p)
        {
            WebManageParameter wmp = (WebManageParameter)p;
            Dao.Open(WebConfigString.DBConn);
            Dao.BeginTrans();
            DBAP dp = ADBAccess.GetDBParametersDefine();
            dp.SQL_Parameters.Add(wmp.Module);
            dp.SQL = @" UPDATE NV_BACK_MST 
                        SET BAKM_CANCEL='Y',BAKM_STS=9
                        WHERE BAKM_NO in (select * from dbo.f_SplitToNvarchar(@DelBAKM_NO,','))";
            Dao.ExecuteNoQuery(dp);
            Dao.Commit();
            Dao.Close();

            //增加log機制，記錄NV_BACK_MST退貨單主檔退貨歷程 YangPeng 2014/9/19 begin
            GlobalCommon.Logger.WriteLog(WITS.Framework.Interface.LoggerLevel.INFO,
                "NV_BACK_MST退貨單主檔表退貨單號:" + ComFunc.nvl(p.GetValue<string>("DelBAKM_NO"))
                + ";狀態為:9;當前操作為:作廢");
            //增加log機制，記錄NV_BACK_MST退貨單主檔退貨歷程 YangPeng 2014/9/19 end
            return true;
        }

        public bool Insert(ParametersStd p)
        {
            WebManageParameter wmp = (WebManageParameter)p;
            DataSet DsAll = p.GetValue<DataSet>("InsertDs");
            Dao.Open(WebConfigString.DBConn);
            int Price_QTY = 0;
            if (p.GetValue<string>("Acction").ToString().Equals("Inert"))
            {
                #region 新增操作

                Dao.BeginTrans();
                Dao.Insert(DsAll, new string[] { "NV_BACK_MST", "NV_BACK_DTL", "NV_BACK_PROD" });
                Dao.Commit();
                Dao.Close();

                //增加log機制，記錄NV_BACK_MST退貨單主檔退貨歷程 YangPeng 2014/9/19 begin
                GlobalCommon.Logger.WriteLog(WITS.Framework.Interface.LoggerLevel.INFO,
                    "NV_BACK_MST退貨單主檔表退貨單號:" + ComFunc.nvl(DsAll.Tables["NV_BACK_MST"].Rows[0]["BAKM_NO"])
                    + ";狀態為:" + ComFunc.nvl(DsAll.Tables["NV_BACK_MST"].Rows[0]["BAKM_STS"]) + ";當前操作為:新增");
                //增加log機制，記錄NV_BACK_MST退貨單主檔退貨歷程 YangPeng 2014/9/19 end
                #endregion
            }
            if (p.GetValue<string>("Acction").ToString().Equals("Update"))
            {
                #region 更新修改操作

                DataSet dt = p.GetValue<DataSet>("UpdateDt"); //一般商品更新信息
                string[] strSamples = p.GetValue<string>("UpSample").Split('/'); //附加收費商品信息
                DataTable dtNV_BACK_DTL = dt.Tables["NV_BACK_DTL"] as DataTable;
                DBAP dp = ADBAccess.GetDBParametersDefine();
                Dao.BeginTrans();
                dp.SQL = "DELETE FROM NV_BACK_DTL WHERE BAKD_NO='" + wmp.Module.GetValue<string>("BAKM_NO") + "'";
                //Dao.ExecuteNoQuery(dp); //刪除所有明細檔案數據
                if (dt.Tables[0].Rows.Count >= 1)
                {
                    dp.SQL = @"UPDATE [NV_BACK_MST]
                           SET [BAKM_DATE] = '" + dt.Tables["NV_BACK_MST"].Rows[0]["BAKM_DATE"] + "'" +
                              @",[BAKM_STOCK] = '" + dt.Tables["NV_BACK_MST"].Rows[0]["BAKM_STOCK"] + "'" +
                              @",[BAKM_CUST] = '" + dt.Tables["NV_BACK_MST"].Rows[0]["BAKM_CUST"] + "'" +
                              @",[BAKM_APPLY_CODE] = '" + dt.Tables["NV_BACK_MST"].Rows[0]["BAKM_APPLY_CODE"] + "'" +
                              @",[BAKM_STS] = '" + dt.Tables["NV_BACK_MST"].Rows[0]["BAKM_STS"] + "'" +
                              @",[ADD_USER_ID] = '" + dt.Tables["NV_BACK_MST"].Rows[0]["ADD_USER_ID"] + "'" +
                              @",[BAKM_DESC] = '" + dt.Tables["NV_BACK_MST"].Rows[0]["BAKM_DESC"] + "'" +
                              @",[ADD_DATE] = '" + dt.Tables["NV_BACK_MST"].Rows[0]["ADD_DATE"] + "'" +
                              @",[ADD_TIME] = '" + dt.Tables["NV_BACK_MST"].Rows[0]["ADD_TIME"] + "'" +
                              @",[UPD_USER_ID] = '" + dt.Tables["NV_BACK_MST"].Rows[0]["UPD_USER_ID"] + "'" +
                              @",[UPD_DATE] = REPLACE(Convert(varchar(10),GetDate(),120),'-','') " +
                              @",[UPD_TIME] = '" + dt.Tables["NV_BACK_MST"].Rows[0]["UPD_TIME"] + "'" +
                              @",[BAKM_RETURN_CODE] = '" + dt.Tables["NV_BACK_MST"].Rows[0]["BAKM_RETURN_CODE"] + "'" +
                         @"WHERE  BAKM_NO='" + dt.Tables["NV_BACK_MST"].Rows[0]["BAKM_NO"] + "' and BAKM_ACCT_YM='" + dt.Tables["NV_BACK_MST"].Rows[0]["BAKM_ACCT_YM"] + "'";
                    Dao.ExecuteNoQuery(dp); //更新主檔表數據


                    //增加log機制，記錄NV_BACK_MST退貨單主檔退貨歷程 YangPeng 2014/9/19 begin
                    GlobalCommon.Logger.WriteLog(WITS.Framework.Interface.LoggerLevel.INFO,
                        "NV_BACK_MST退貨單主檔表退貨單號:" + ComFunc.nvl(dt.Tables["NV_BACK_MST"].Rows[0]["BAKM_NO"])
                        + ";狀態為:" + ComFunc.nvl(dt.Tables["NV_BACK_MST"].Rows[0]["BAKM_STS"]) + ";當前操作為:存檔");
                    //增加log機制，記錄NV_BACK_MST退貨單主檔退貨歷程 YangPeng 2014/9/19 end
                }
                //Dao.Insert(dtNV_BACK_DTL, "NV_BACK_DTL"); //新增明細檔案數據
                for (int i = 0; i < strSamples.Length; i++)
                {
                    dp.SQL = @"update NV_BACK_PROD
                            set
                            BAKP_QTY=" + Convert.ToDouble(strSamples[i].Split(',')[1].ToString()) +
                            @" where BAKP_NO='" + dt.Tables[0].Rows[0]["BAKM_NO"] + "'" +
                            @" and BAKP_PROD='" + strSamples[i].Split(',')[0].ToString() + "'";
                    Dao.ExecuteNoQuery(dp); //更新收費檔案數據
                }
                Dao.Commit();
                Dao.Close();

                #endregion
            }
            if (p.GetValue<string>("Acction").ToString().Equals("Affirm"))
            {
                Dao.BeginTrans();
                DBAP dbap = ADBAccess.GetDBParametersDefine();

                #region 參數

                string[] strSample = p.GetValue<string>("strSample").ToString().Split(',');//样品ID
                string[] strSampleprice = p.GetValue<string>("strSampleprice").ToString().Split(',');//样品价格
                string strYear = wmp.Module.GetValue<string>("BAKM_ACCT_YM").Replace("/", "");//帳務年月
                string strStock = p.GetValue<string>("strStock");//倉庫
                string strCust_StockID = p.GetValue<string>("strCust_StockID");
                string[] strPoudID = p.GetValue<string>("strPoudID").Split(',');//商品ID
                string[] strPoudNum = p.GetValue<string>("strPoudNum").Split(',');//商品數量
                string[] strATTR = p.GetValue<string>("strATTR").Split(',');//商品屬性
                string[] strPoudPrice = p.GetValue<string>("strPoudPrice").Split(',');
                string strOdd = wmp.Module.GetValue<string>("strOdd").ToString();

                #endregion

                #region 樣品庫存處理

                if (p.GetValue<string>("strSample").ToString() != "")  //樣品信息
                {
                    for (int i = 0; i < strSample.Length; i++)
                    {
                        dbap.SQL = @"update NV_SAMPLE_MONTHLY_DATA 
                                    set SPMD_ALREADY_GRANT_AMT=SPMD_ALREADY_GRANT_AMT+" + Convert.ToDouble(strSampleprice[i]) +
                                    @"where SPMD_PERIOD=
                                    (select Field_No from NV_CODE_TABLE 
                                    where Type_No='015' and Field_Content='" + strYear.Substring(4, 2) +
                                    @"') and SPMD_YEAR='" + strYear.Substring(0, 4) + "'";
                        Dao.ExecuteNoQuery(dbap);
                    }
                }

                #endregion

                #region    将退货主档表的STS改为4並且退貨單是否確認改為Y

                dbap.SQL = @" update NV_BACK_MST 
                              set BAKM_STS=4 
                             ,BAKM_CONFIRM='Y' 
                             where BAKM_NO='" + wmp.Module.GetValue<string>("BAKM_NO") + "'";
                Dao.ExecuteNoQuery(dbap);


                //增加log機制，記錄NV_BACK_MST退貨單主檔退貨歷程 YangPeng 2014/9/19 begin
                GlobalCommon.Logger.WriteLog(WITS.Framework.Interface.LoggerLevel.INFO,
                    "NV_BACK_MST退貨單主檔表退貨單號:" + ComFunc.nvl(wmp.Module.GetValue<string>("BAKM_NO"))
                    + ";狀態為:4;當前操作為:確認");
                //增加log機制，記錄NV_BACK_MST退貨單主檔退貨歷程 YangPeng 2014/9/19 end
                #endregion

                #region 商品庫存

                #region 依據數據加回商品庫存倉庫&&庫存異動記錄檔&&倉庫進銷存資料檔
                //NVRU-377 原邏輯，將prodid拼接之後再分割，現在直接取網格中的數據 begin YangPeng 2014/12/18
                DataTable dtNV_BACK_DTL_temp = wmp.GetValue<DataTable>("Confirm_BACK_DTL");
                for (int i = 0; i < dtNV_BACK_DTL_temp.Rows.Count; i++)
                {
                    string prodid = dtNV_BACK_DTL_temp.Rows[i]["BAKD_PROD"].ToString();
                    string AccYM = strYear.ToString();
                    string Stock = strStock.ToString();
                    double qty = Convert.ToDouble(dtNV_BACK_DTL_temp.Rows[i]["BAKD_QTY"].ToString());

                    dbap.SQL = @"BEGIN if exists( SELECT * FROM NV_PROD_STOCK WHERE PDST_STCK_CODE ='" + Stock + "' AND PDST_PROD_ID ='" + prodid + "') update NV_PROD_STOCK set PDST_GOOD_QTY=PDST_GOOD_QTY+" + qty + " where PDST_STCK_CODE='" + Stock + "' and PDST_PROD_ID='" + prodid + "'ELSE INSERT INTO NV_PROD_STOCK (PDST_PROD_ID,PDST_STCK_CODE,PDST_GOOD_QTY) values('" + prodid + "','" + Stock + "'," + qty + ") END ";
                    Dao.ExecuteNoQuery(dbap);
                    GlobalCommon.Logger.WriteLog(WITS.Framework.Interface.LoggerLevel.DEBUG, dbap.SQL + "(1)" + "退貨單主檔表退貨單號:" + strOdd);

                    dbap.SQL = @"insert into dbo.NV_PROD_REC(PDRC_YM, PDRC_NO, PDRC_TYPE, PDRC_PROD, PDRC_GOOD_OR_NG, PDRC_STOCK, PDRC_DATE, PDRC_QTY)" +
                               @"values(" +
                                    @"'" + AccYM + "','" + wmp.Module.GetValue<string>("BAKM_NO") +
                                    @"','R', '" + prodid + "','G','" + Stock +
                                    @"',convert(varchar(8),GETDATE(),112),'" + qty + "')";
                    Dao.ExecuteNoQuery(dbap); //庫存異動記錄檔
                    GlobalCommon.Logger.WriteLog(WITS.Framework.Interface.LoggerLevel.DEBUG, dbap.SQL + "(2)" + "退貨單主檔表退貨單號:" + strOdd);



                    dbap.SQL = @"select * from NV_CODE_TABLE where Type_No='018' and Field_Content='" + prodid + "'";
                    DataSetStd dts1 = Dao.Query(dbap);
                    DataTable dt1 = dts1.Tables[0];
                    if (dt1.Rows.Count == 0) //如有數據
                    {
                        dbap.SQL = @"select * from NV_PROD_TRANS where PDTN_YM='" + AccYM + "' and PDTN_STOCK_CODE='" + Stock + "'  and PDTN_PROD_CODE='" + prodid + "'";
                        DataSetStd dts2 = Dao.Query(dbap);
                        DataTable dt2 = dts2.Tables[0];
                        if (dt2.Rows.Count > 0)
                        {
                            dbap.SQL = @" update NV_PROD_TRANS set PDTN_RTN_GOOD_QTY=PDTN_RTN_GOOD_QTY+" + qty + ", PDTN_END_GOOD_QTY=PDTN_END_GOOD_QTY+" + qty + " where PDTN_YM='" + AccYM + "' and PDTN_STOCK_CODE='" + Stock + "'  and PDTN_PROD_CODE='" + prodid + "'";
                        }
                        else
                        {
                            dbap.SQL = @"select * from NV_PROD_TRANS where PDTN_YM=(substring(convert(varchar(8),DateAdd(Month,-1,convert(date,'" + AccYM + "'+'01')),112),1,6)) and PDTN_STOCK_CODE='" + Stock + "' and PDTN_PROD_CODE='" + prodid + "'";
                            DataSetStd dts3 = Dao.Query(dbap);
                            DataTable dt3 = dts3.Tables[0];
                            if (dt3.Rows.Count > 0)
                            {
                                dbap.SQL = @" insert into NV_PROD_TRANS(PDTN_YM,PDTN_STOCK_CODE,PDTN_PROD_CODE,PDTN_BGN_GOOD_QTY,PDTN_RTN_GOOD_QTY,PDTN_END_GOOD_QTY)
                                                select top 1 '" + AccYM + "','" + Stock + "','" + prodid + "',PDTN_END_GOOD_QTY," + qty + ",PDTN_END_GOOD_QTY+" + qty + " from NV_PROD_TRANS where PDTN_YM=(substring(convert(varchar(8),DateAdd(Month,-1,convert(date,'" + AccYM + "'+'01')),112),1,6)) and PDTN_STOCK_CODE='" + Stock + "' and PDTN_PROD_CODE='" + prodid + "' order by PDTN_YM desc";
                            }
                            else
                            {
                                dbap.SQL = @" insert into NV_PROD_TRANS(PDTN_YM,PDTN_STOCK_CODE,PDTN_PROD_CODE,PDTN_BGN_GOOD_QTY,PDTN_RTN_GOOD_QTY,PDTN_END_GOOD_QTY)
                                                values('" + AccYM + "','" + Stock + "','" + prodid + "',0," + qty + "," + qty + ") ";
                            }
                        }
                    }

                    Dao.ExecuteNoQuery(dbap);
                    GlobalCommon.Logger.WriteLog(WITS.Framework.Interface.LoggerLevel.DEBUG, dbap.SQL + "(3)" + "退貨單主檔表退貨單號:" + strOdd);
                    dbap.SQL = @"update NV_PROD_TRANS set PDTN_BGN_GOOD_QTY=PDTN_BGN_GOOD_QTY+" + qty + ",PDTN_END_GOOD_QTY=PDTN_END_GOOD_QTY+" + qty + " where PDTN_YM>'" + AccYM + "' and PDTN_STOCK_CODE='" + Stock + "' and PDTN_PROD_CODE='" + prodid + "'";
                    Dao.ExecuteNoQuery(dbap);
                    GlobalCommon.Logger.WriteLog(WITS.Framework.Interface.LoggerLevel.DEBUG, dbap.SQL + "(4)" + "退貨單主檔表退貨單號:" + strOdd);
                }
                //NVRU-377 原邏輯，將prodid拼接之後再分割，現在直接取網格中的數據 end YangPeng 2014/12/18
                #region 原邏輯，將prodid拼接之後再分割，現在直接取網格中的數據
                /*
                for (int i = 0; i < strPoudID.Length; i++)
                {
//                    dbap.SQL = @"update NV_PROD_STOCK set
//                                PDST_GOOD_QTY=PDST_GOOD_QTY+" + Convert.ToDouble(strPoudNum[i]) +
//                                @" where PDST_STCK_CODE='" + strStock +
//                                @"' and PDST_PROD_ID='" + strPoudID[i] + "'";
//                    Dao.ExecuteNoQuery(dbap); //依據數據加回商品庫存倉庫

                    dbap.SQL = @"BEGIN if exists( SELECT * FROM NV_PROD_STOCK WHERE PDST_STCK_CODE ='" + strStock + "' AND PDST_PROD_ID ='" + strPoudID[i] + "') update NV_PROD_STOCK set PDST_GOOD_QTY=PDST_GOOD_QTY+" + Convert.ToDouble(strPoudNum[i]) + " where PDST_STCK_CODE='" + strStock + "' and PDST_PROD_ID='" + strPoudID[i] + "'ELSE INSERT INTO NV_PROD_STOCK (PDST_PROD_ID,PDST_STCK_CODE,PDST_GOOD_QTY) values('" + strPoudID[i] + "','" + strStock + "'," + Convert.ToDouble(strPoudNum[i]) + ") END ";
                    Dao.ExecuteNoQuery(dbap);
                    GlobalCommon.Logger.WriteLog(WITS.Framework.Interface.LoggerLevel.DEBUG, dbap.SQL + "(1)"+"退貨單主檔表退貨單號:" + strOdd);

                    dbap.SQL = @"insert into dbo.NV_PROD_REC(PDRC_YM, PDRC_NO, PDRC_TYPE, PDRC_PROD, PDRC_GOOD_OR_NG, PDRC_STOCK, PDRC_DATE, PDRC_QTY)" +
                               @"values(" +
                                    @"'" + strYear + "','" + wmp.Module.GetValue<string>("BAKM_NO") +
                                    @"','R', '" + strPoudID[i] + "','G','" + strStock +
                                    @"',convert(varchar(8),GETDATE(),112),'" + Convert.ToDouble(strPoudNum[i]) + "')";
                    Dao.ExecuteNoQuery(dbap); //庫存異動記錄檔
                    GlobalCommon.Logger.WriteLog(WITS.Framework.Interface.LoggerLevel.DEBUG, dbap.SQL + "(2)" + "退貨單主檔表退貨單號:" + strOdd);
                    //dbap.SQL = @"update NV_PROD_TRANS set PDTN_RTN_GOOD_QTY=PDTN_RTN_GOOD_QTY+" + Convert.ToDouble(strPoudNum[i]) +
                    //            @", PDTN_END_GOOD_QTY=PDTN_END_GOOD_QTY+" + Convert.ToDouble(strPoudNum[i]) +
                    //            @" where PDTN_YM='" + strYear +
                    //            @"' and PDTN_PROD_CODE='" + strPoudID[i] +
                    //            @"' and PDTN_STOCK_CODE='" + strStock + "'";
                    //Dao.ExecuteNoQuery(dbap);//倉庫進銷存資料檔


                    string prodid = strPoudID[i].ToString();
                    string AccYM = strYear.ToString();
                    string Stock = strStock.ToString();
                    double qty = Convert.ToDouble(strPoudNum[i]);

                    
                    dbap.SQL = @"select * from NV_CODE_TABLE where Type_No='018' and Field_Content='" + prodid + "'";
                    DataSetStd dts1 = Dao.Query(dbap);
                    DataTable dt1 = dts1.Tables[0];
                    if (dt1.Rows.Count == 0) //如有數據
                    {
                        dbap.SQL = @"select * from NV_PROD_TRANS where PDTN_YM='" + AccYM + "' and PDTN_STOCK_CODE='" + Stock + "'  and PDTN_PROD_CODE='" + prodid + "'";
                        DataSetStd dts2 = Dao.Query(dbap);
                        DataTable dt2 = dts2.Tables[0];
                        if (dt2.Rows.Count > 0)
                        {
                            dbap.SQL = @" update NV_PROD_TRANS set PDTN_RTN_GOOD_QTY=PDTN_RTN_GOOD_QTY+" + qty + ", PDTN_END_GOOD_QTY=PDTN_END_GOOD_QTY+" + qty + " where PDTN_YM='" + AccYM + "' and PDTN_STOCK_CODE='" + Stock + "'  and PDTN_PROD_CODE='" + prodid + "'";
                        }
                        else
                        {
                            dbap.SQL = @"select * from NV_PROD_TRANS where PDTN_YM=(substring(convert(varchar(8),DateAdd(Month,-1,convert(date,'" + AccYM + "'+'01')),112),1,6)) and PDTN_STOCK_CODE='" + Stock + "' and PDTN_PROD_CODE='" + prodid + "'";
                            DataSetStd dts3 = Dao.Query(dbap);
                            DataTable dt3 = dts3.Tables[0];
                            if (dt3.Rows.Count > 0)
                            {
                                dbap.SQL = @" insert into NV_PROD_TRANS(PDTN_YM,PDTN_STOCK_CODE,PDTN_PROD_CODE,PDTN_BGN_GOOD_QTY,PDTN_RTN_GOOD_QTY,PDTN_END_GOOD_QTY)
                                                select top 1 '" + AccYM + "','" + Stock + "','" + prodid + "',PDTN_END_GOOD_QTY," + qty + ",PDTN_END_GOOD_QTY+" + qty + " from NV_PROD_TRANS where PDTN_YM=(substring(convert(varchar(8),DateAdd(Month,-1,convert(date,'" + AccYM + "'+'01')),112),1,6)) and PDTN_STOCK_CODE='" + Stock + "' and PDTN_PROD_CODE='" + prodid + "' order by PDTN_YM desc";
                            }
                            else
                            {
                                dbap.SQL = @" insert into NV_PROD_TRANS(PDTN_YM,PDTN_STOCK_CODE,PDTN_PROD_CODE,PDTN_BGN_GOOD_QTY,PDTN_RTN_GOOD_QTY,PDTN_END_GOOD_QTY)
                                                values('" + AccYM + "','" + Stock + "','" + prodid + "',0," + qty + "," + qty + ") ";
                            }
                        }
                    }

                           Dao.ExecuteNoQuery(dbap);
                           GlobalCommon.Logger.WriteLog(WITS.Framework.Interface.LoggerLevel.DEBUG, dbap.SQL + "(3)" + "退貨單主檔表退貨單號:" + strOdd);
                           dbap.SQL = @"update NV_PROD_TRANS set PDTN_BGN_GOOD_QTY=PDTN_BGN_GOOD_QTY+" + qty + ",PDTN_END_GOOD_QTY=PDTN_END_GOOD_QTY+" + qty + " where PDTN_YM>'" + AccYM + "' and PDTN_STOCK_CODE='" + Stock + "' and PDTN_PROD_CODE='" + prodid + "'";
                           Dao.ExecuteNoQuery(dbap);
                           GlobalCommon.Logger.WriteLog(WITS.Framework.Interface.LoggerLevel.DEBUG, dbap.SQL + "(4)" + "退貨單主檔表退貨單號:" + strOdd);
//                    string strNV_Prod_Trans_Update2 = @"if not exists(select * from NV_CODE_TABLE where Type_No='018' and Field_Content=@prodid)
//                                            begin
//                                                if exists(select * from NV_PROD_TRANS where PDTN_YM=@AccYM and PDTN_STOCK_CODE=@Stock and PDTN_PROD_CODE=@prodid)
//                                                update NV_PROD_TRANS set PDTN_RTN_GOOD_QTY=PDTN_RTN_GOOD_QTY+@qty, PDTN_END_GOOD_QTY=PDTN_END_GOOD_QTY+@qty where  PDTN_YM=@AccYM and PDTN_STOCK_CODE=@Stock and PDTN_PROD_CODE=@prodid
//                                                    else if exists(select * from NV_PROD_TRANS where PDTN_YM=(substring(convert(varchar(8),DateAdd(Month,-1,convert(date,@AccYM+'01')),112),1,6)) and PDTN_STOCK_CODE=@Stock and PDTN_PROD_CODE=@prodid)
//                                                insert into NV_PROD_TRANS(PDTN_YM,PDTN_STOCK_CODE,PDTN_PROD_CODE,PDTN_BGN_GOOD_QTY,PDTN_RTN_GOOD_QTY,PDTN_END_GOOD_QTY)
//                                                select top 1 @AccYM,@Stock,@prodid,PDTN_END_GOOD_QTY,@qty,PDTN_END_GOOD_QTY+@qty
//                                                from NV_PROD_TRANS
//                                                where PDTN_YM=(substring(convert(varchar(8),DateAdd(Month,-1,convert(date,@AccYM+'01')),112),1,6)) and PDTN_STOCK_CODE=@Stock and PDTN_PROD_CODE=@prodid
//                                                order by PDTN_YM desc
//                                                    else
//                                                insert into NV_PROD_TRANS(PDTN_YM,PDTN_STOCK_CODE,PDTN_PROD_CODE,PDTN_BGN_GOOD_QTY,PDTN_RTN_GOOD_QTY,PDTN_END_GOOD_QTY)
//                                                values(@AccYM,@Stock,@prodid,0,@qty,@qty)
//
//                                                update NV_PROD_TRANS set PDTN_BGN_GOOD_QTY=PDTN_BGN_GOOD_QTY+@qty,PDTN_END_GOOD_QTY=PDTN_END_GOOD_QTY+@qty
//                                                where PDTN_YM>@AccYM and PDTN_STOCK_CODE=@Stock and PDTN_PROD_CODE=@prodid
//                                            end";
//                    dbap.SQL = strNV_Prod_Trans_Update2;
//                    dbap.SQL_Parameters.Add("prodid", strPoudID[i]);
//                    dbap.SQL_Parameters.Add("AccYM",strYear);
//                    dbap.SQL_Parameters.Add("Stock",strStock);
//                    dbap.SQL_Parameters.Add("qty",Convert.ToDouble(strPoudNum[i]) );
//                    Dao.ExecuteNoQuery(dbap); 
                }
                 * */
                #endregion


                #endregion

                #endregion

                #region 代理店庫存

                for (int i = 0; i < strATTR.Length; i++)
                {
                    #region 查詢對應的代理店庫存信息進行庫存異動


                    dbap.SQL = @"select CSST_CUST_ID,CSST_PROD_ID,CSST_ATTR_YM,CSST_PRICE,CSST_SAL_ATTR,sum(CSST_QTY) as CSST_QTY
                                    from NV_CUST_STOCK 
                                    where  CSST_CUST_ID='" + strCust_StockID +
                                    @"' and CSST_SAL_ATTR='" + strATTR[i] +
                                    @"' and CSST_PROD_ID='" + strPoudID[i] +
                                    @"' and CSST_PRICE='" + strPoudPrice[i] +
                                    @"'group by CSST_CUST_ID,CSST_PROD_ID,CSST_ATTR_YM,CSST_PRICE,CSST_SAL_ATTR
                                            order by CSST_ATTR_YM asc";
                    DataSetStd dts = Dao.Query(dbap);
                    DataTable dtStock = dts.Tables[0];

                    #endregion

                    #region 找到相應的庫存信息，從小到大開始處理代理店庫存

                    Double QTY = Convert.ToDouble(strPoudNum[i]);
                    foreach (DataRow item in dtStock.Rows)
                    {
                        if (Convert.ToDouble(item["CSST_QTY"]) >= QTY)  //如果代理店庫存大於退貨數量就退出循環
                        {
                            #region NV_代理店庫存檔(NV_CUST_STOCK

                            dbap.SQL = @"update NV_CUST_STOCK set
                                            CSST_QTY=CSST_QTY-" + QTY +
                                            @"where  CSST_CUST_ID='" + strCust_StockID +
                                            @"' and CSST_SAL_ATTR='" + strATTR[i] +
                                            @"' and CSST_PROD_ID='" + strPoudID[i] +
                                            @"' and CSST_PRICE='" + strPoudPrice[i] + "' and CSST_ATTR_YM='" + item["CSST_ATTR_YM"] + "'";
                            Dao.ExecuteNoQuery(dbap);

                            #endregion

                            #region NV_代理店進銷存資料檔 (NV_CUST_TRANS

                            dbap.SQL = //@"if exists(select * from NV_CUST_TRANS where CSTN_CUST_ID='" + item["CSST_CUST_ID"] + "' and CSTN_PROD_ID='" + item["CSST_PROD_ID"] + "' and CSTN_ATTR_YM='" + item["CSST_ATTR_YM"] + "' and CSTN_PRICE=" + item["CSST_PRICE"].ToString() + " and CSTN_SAL_ATTR='" + item["CSST_SAL_ATTR"] + "' and CSTN_YM='" + strYear + "')" +
                                            @"UPDATE NV_CUST_TRANS 
                                            set CSTN_RTN_QTY=CSTN_RTN_QTY+" + QTY +
                                            @", CSTN_END_QTY=CSTN_END_QTY-" + QTY +
                                            @" where CSTN_CUST_ID='" + item["CSST_CUST_ID"] +
                                            @"'and CSTN_PROD_ID='" + item["CSST_PROD_ID"] +
                                            @"'and CSTN_ATTR_YM='" + item["CSST_ATTR_YM"] +
                                            @"'and CSTN_PRICE=" + Convert.ToDouble(item["CSST_PRICE"]) +
                                            @" and CSTN_SAL_ATTR='" + item["CSST_SAL_ATTR"] +
                                            @"'and CSTN_YM='" + strYear + "';" +
                                            @"update NV_CUST_TRANS 
                                            set CSTN_BGN_QTY=CSTN_BGN_QTY-" + QTY +
                                            @", CSTN_END_QTY=CSTN_END_QTY-" + QTY +
                                            @" where CSTN_CUST_ID='" + item["CSST_CUST_ID"] +
                                            @"'and CSTN_PROD_ID='" + item["CSST_PROD_ID"] +
                                            @"'and CSTN_ATTR_YM='" + item["CSST_ATTR_YM"] +
                                            @"'and CSTN_PRICE=" + Convert.ToDouble(item["CSST_PRICE"]) +
                                            @" and CSTN_SAL_ATTR='" + item["CSST_SAL_ATTR"] +
                                            @"'and CSTN_YM>'" + strYear + "'";
                            //@" else
                            //insert into NV_CUST_TRANS(CSTN_CUST_ID,CSTN_PROD_ID,CSTN_ATTR_YM,CSTN_PRICE,CSTN_SAL_ATTR,CSTN_YM,CSTN_RTN_QTY,CSTN_END_QTY,CSTN_BGN_QTY)
                            //values('" + item["CSST_CUST_ID"] + "','" + item["CSST_PROD_ID"] + "','" + item["CSST_ATTR_YM"] + "'," + item["CSST_PRICE"] + ",'" + item["CSST_SAL_ATTR"] + "','" + strYear + "'," + QTY + "," + QTY * (-1) + "," + 0 + ")";
                            Dao.ExecuteNoQuery(dbap);

                            #endregion

                            #region NV_代理店庫存異動記錄檔(NV_CUST_REC)

                            dbap.SQL = @"insert into 
                                        NV_CUST_REC(CSRC_YM, CSRC_NO, CSRC_TYPE,
                                        CSRC_PROD, CSRC_PRICE, CSRC_SAL_ATTR, 
                                        CSRC_ATTR_YM, CSRC_CUST_ID, CSRC_DATE, CSRC_QTY) 
                                        values('" + strYear + "','" + wmp.Module.GetValue<string>("BAKM_NO") + "','R'," +
                                        @"'" + strPoudID[i] + "','" + Convert.ToDouble(strPoudPrice[i]) + "','" + strATTR[i] +
                                        @"','" + item["CSST_ATTR_YM"] + "','" + strCust_StockID + "',convert(varchar(8),GETDATE(),112),'" + QTY +
                                        @"')";
                            Dao.ExecuteNoQuery(dbap);

                            #endregion

                            break;
                        }
                        else if (Convert.ToDouble(item["CSST_QTY"]) < QTY)                                                                        //如果代理店庫存小於退貨數量
                        {
                            #region NV_代理店庫存檔(NV_CUST_STOCK

                            dbap.SQL = @"update NV_CUST_STOCK set CSST_QTY=0" +
                                             @" where  CSST_CUST_ID='" + strCust_StockID +
                                             @"' and CSST_SAL_ATTR='" + strATTR[i] +
                                             @"' and CSST_PROD_ID='" + strPoudID[i] +
                                             @"' and CSST_PRICE='" + strPoudPrice[i] + "' and CSST_ATTR_YM='" + item["CSST_ATTR_YM"] + "'";
                            Dao.ExecuteNoQuery(dbap);

                            #endregion

                            #region NV_代理店進銷存資料檔 (NV_CUST_TRANS

                            dbap.SQL = //@"if exists(select * from NV_CUST_TRANS where CSTN_CUST_ID='" + item["CSST_CUST_ID"] + "' and CSTN_PROD_ID='" + item["CSST_PROD_ID"] + "' and CSTN_ATTR_YM='" + item["CSST_ATTR_YM"] + "' and CSTN_PRICE=" + item["CSST_PRICE"].ToString() + " and CSTN_SAL_ATTR='" + item["CSST_SAL_ATTR"] + "' and CSTN_YM='" + strYear + "')" +
                                            @"UPDATE NV_CUST_TRANS 
                                            SET CSTN_RTN_QTY=CSTN_RTN_QTY+(" + Convert.ToDouble(item["CSST_QTY"]) +
                                            @"), CSTN_END_QTY=CSTN_END_QTY-(" + Convert.ToDouble(item["CSST_QTY"]) +
                                            @") WHERE CSTN_CUST_ID='" + item["CSST_CUST_ID"] +
                                            @"'AND CSTN_PROD_ID='" + item["CSST_PROD_ID"] +
                                            @"'AND CSTN_ATTR_YM='" + item["CSST_ATTR_YM"] +
                                            @"'AND CSTN_PRICE=" + Convert.ToDouble(item["CSST_PRICE"]) +
                                            @" AND CSTN_SAL_ATTR='" + item["CSST_SAL_ATTR"] +
                                            @"'AND CSTN_YM='" + strYear + "';" +
                                            @"UPDATE NV_CUST_TRANS 
                                            set CSTN_BGN_QTY=CSTN_BGN_QTY-(" + Convert.ToDouble(item["CSST_QTY"]) +
                                            @"), CSTN_END_QTY=CSTN_END_QTY-(" + Convert.ToDouble(item["CSST_QTY"]) +
                                            @") WHERE CSTN_CUST_ID='" + item["CSST_CUST_ID"] +
                                            @"'AND CSTN_PROD_ID='" + item["CSST_PROD_ID"] +
                                            @"'AND CSTN_ATTR_YM='" + item["CSST_ATTR_YM"] +
                                            @"'AND CSTN_PRICE=" + Convert.ToDouble(item["CSST_PRICE"]) +
                                            @" AND CSTN_SAL_ATTR='" + item["CSST_SAL_ATTR"] +
                                            @"'AND CSTN_YM>'" + strYear + "'";
                            //@" else
                            //insert into NV_CUST_TRANS(CSTN_CUST_ID,CSTN_PROD_ID,CSTN_ATTR_YM,CSTN_PRICE,CSTN_SAL_ATTR,CSTN_YM,CSTN_RTN_QTY,CSTN_END_QTY,CSTN_BGN_QTY)
                            //values('" + item["CSST_CUST_ID"] + "','" + item["CSST_PROD_ID"] + "','" + item["CSST_ATTR_YM"] + "'," + item["CSST_PRICE"] + ",'" + item["CSST_SAL_ATTR"] + "','" + strYear + "'," + Convert.ToDouble(item["CSST_QTY"]) + "," + Convert.ToDouble(item["CSST_QTY"]) * (-1) + "," + 0 + ")";
                            Dao.ExecuteNoQuery(dbap);

                            #region NV_代理店庫存異動記錄檔(NV_CUST_REC)

                            if (Convert.ToInt64(item["CSST_QTY"]) != 0)
                            {
                                dbap.SQL = @"insert into 
                                        NV_CUST_REC(CSRC_YM, CSRC_NO, CSRC_TYPE, 
                                        CSRC_PROD, CSRC_PRICE, CSRC_SAL_ATTR, 
                                        CSRC_ATTR_YM, CSRC_CUST_ID, CSRC_DATE, CSRC_QTY) 
                                        values('" + strYear + "','" + wmp.Module.GetValue<string>("BAKM_NO") + "','R'," +
                                            @"'" + strPoudID[i] + "','" + Convert.ToDouble(strPoudPrice[i]) + "','" + strATTR[i] +
                                            @"','" + item["CSST_ATTR_YM"] + "','" + strCust_StockID + "',convert(varchar(8),GETDATE(),112),'" + Convert.ToDouble(item["CSST_QTY"]) +
                                            @"')";
                                Dao.ExecuteNoQuery(dbap);
                            }

                            #endregion


                            QTY = QTY - Convert.ToDouble(item["CSST_QTY"]);
                            #endregion
                        }
                    }
                    #endregion
                }

                #endregion

                //add by LuYang nvr-925
                dbap.SQL = @"delete from NV_BACK_DTL where BAKD_NO='" + wmp.Module.GetValue<string>("BAKM_NO") + "'";
                Dao.ExecuteNoQuery(dbap);
                DataTable dtNV_BACK_DTL = wmp.GetValue<DataTable>("Confirm_BACK_DTL");
                Dao.Insert(dtNV_BACK_DTL, "NV_BACK_DTL"); //新增明細檔案數據
                //end of add

                //add by yanglu 附加商品
                string strNV_BAKD_PROD_Update = @"update NV_BACK_PROD set BAKP_QTY=@qty  where BAKP_NO=@Number and BAKP_PROD=@prodid";
                string strNV_Prod_Stock_Update = @"if not exists(select * from NV_CODE_TABLE where Type_No='018' and Field_Content=@prodid)
                                                   update NV_PROD_STOCK 
                                                   set PDST_GOOD_QTY=PDST_GOOD_QTY-@qty 
                                                   where PDST_STCK_CODE=@stock 
                                                   and PDST_PROD_ID=@prodid";
                string strNV_Prod_Trans_Update = @"if not exists(select * from NV_CODE_TABLE where Type_No='018' and Field_Content=@prodid)
                                            begin
                                                if exists(select * from NV_PROD_TRANS where PDTN_YM=@AccYM and PDTN_STOCK_CODE=@Stock and PDTN_PROD_CODE=@prodid)
                                                update NV_PROD_TRANS set PDTN_SAL_QTY=PDTN_SAL_QTY+@qty, PDTN_END_GOOD_QTY=PDTN_END_GOOD_QTY-@qty where  PDTN_YM=@AccYM and PDTN_STOCK_CODE=@Stock and PDTN_PROD_CODE=@prodid
                                                    else if exists(select * from NV_PROD_TRANS where PDTN_YM=(substring(convert(varchar(8),DateAdd(Month,-1,convert(date,@AccYM+'01')),112),1,6)) and PDTN_STOCK_CODE=@Stock and PDTN_PROD_CODE=@prodid)
                                                insert into NV_PROD_TRANS(PDTN_YM,PDTN_STOCK_CODE,PDTN_PROD_CODE,PDTN_BGN_GOOD_QTY,PDTN_SAL_QTY,PDTN_END_GOOD_QTY)
                                                select top 1 @AccYM,@Stock,@prodid,PDTN_END_GOOD_QTY,@qty,PDTN_END_GOOD_QTY-@qty
                                                from NV_PROD_TRANS
                                                where PDTN_YM=(substring(convert(varchar(8),DateAdd(Month,-1,convert(date,@AccYM+'01')),112),1,6)) and PDTN_STOCK_CODE=@Stock and PDTN_PROD_CODE=@prodid
                                                order by PDTN_YM desc
                                                    else
                                                insert into NV_PROD_TRANS(PDTN_YM,PDTN_STOCK_CODE,PDTN_PROD_CODE,PDTN_BGN_GOOD_QTY,PDTN_SAL_QTY,PDTN_END_GOOD_QTY)
                                                values(@AccYM,@Stock,@prodid,0,@qty,(-1)*@qty)

                                                update NV_PROD_TRANS set PDTN_BGN_GOOD_QTY=PDTN_BGN_GOOD_QTY-@qty,PDTN_END_GOOD_QTY=PDTN_END_GOOD_QTY-@qty
                                                where PDTN_YM>@AccYM and PDTN_STOCK_CODE=@Stock and PDTN_PROD_CODE=@prodid
                                            end ";
                string strNV_Prod_Rec_Update = @"if not exists(select * from NV_CODE_TABLE where Type_No='018' and Field_Content=@prodid)
                                            begin
                                                if exists(select * from NV_PROD_REC where PDRC_YM=@AccYM and PDRC_NO=@Number and PDRC_TYPE='S' and PDRC_PROD=@prodid and PDRC_GOOD_OR_NG='G' and PDRC_QTY+@qty=0)
                                                delete from NV_PROD_REC where PDRC_YM=@AccYM and PDRC_NO=@Number and PDRC_TYPE='S' and PDRC_PROD=@prodid and PDRC_GOOD_OR_NG='G'
                                                   else if exists(select * from NV_PROD_REC where PDRC_YM=@AccYM and PDRC_NO=@Number and PDRC_TYPE='S' and PDRC_PROD=@prodid and PDRC_GOOD_OR_NG='G' and PDRC_QTY+@qty<>0)
                                                update NV_PROD_REC set PDRC_QTY=PDRC_QTY+@qty where PDRC_YM=@AccYM and PDRC_NO=@Number and PDRC_TYPE='S' and PDRC_PROD=@prodid and PDRC_GOOD_OR_NG='G'
                                                   else
                                                insert into NV_PROD_REC(PDRC_YM,PDRC_NO,PDRC_TYPE,PDRC_PROD,PDRC_GOOD_OR_NG,PDRC_STOCK,PDRC_DATE,PDRC_QTY)
                                                values(@AccYM,@Number,'S',@prodid,'G',@Stock,convert(varchar(8),getDate(),112),@qty)
                                            end";
                string strNV_Sale_Mst_Insert = @"insert into NV_SALE_MST(SALM_NO,SALM_ORDER_DATE,SALM_DATE,SALM_ACCT_YM,SALM_STOCK,SALM_CUST,SALM_RECEV,SALM_ZIP,SALM_ADDR,SALM_TEL,SALM_CONFIRM,SALM_CANCEL,ADD_USER_ID,ADD_DATE,ADD_TIME,SALM_DESC,SALM_CHARGE)
                                                select @NewNumber,convert(varchar(8),getDate(),112),convert(varchar(8),getDate(),112),@AccYM,@Stock,@CustID,CUST_NAME,isnull(CUST_SHIP_ZIP,''),isnull(CUST_SHIP_ADDR,''),isnull(CUST_SHIP_TEL,''),'Y','N',@ADD_USER_ID,@ADD_DATE,@ADD_TIME,'請勿出貨',0
                                                from nv_custom where cust_id=@CustID";
                string strNV_Sale_Dtl_Insert = @"insert into NV_SALE_DTL(SALD_NO,SALD_PROD,SALD_SEQ,SALD_QTY,SALD_PRICE,SALD_SAL_ATTR,SALD_DESC)
                                                values(@NewNumber,@prodid,@seq,@qty,@price,'A','')";
                string strSql_Cust_Stock = @"if exists(select * from NV_CUST_STOCK where CSST_CUST_ID=@CustID and CSST_PROD_ID=@prodid and CSST_ATTR_YM=@AccYM and CSST_PRICE=@price and CSST_SAL_ATTR='A')
                                            Update NV_CUST_STOCK set CSST_QTY=CSST_QTY+@qty where CSST_CUST_ID=@CustID and CSST_PROD_ID=@prodid and CSST_ATTR_YM=@AccYM and CSST_PRICE=@price and CSST_SAL_ATTR='A'
                                               else
                                            insert into NV_CUST_STOCK(CSST_CUST_ID,CSST_PROD_ID,CSST_ATTR_YM,CSST_PRICE,CSST_SAL_ATTR,CSST_QTY)
                                            values(@CustID,@prodid,@AccYM,@price,'A',@qty)";
                string strSql_Cust_Rec = @"if exists(select * from NV_CUST_REC where CSRC_YM=@AccYM and CSRC_NO=@NewNumber and CSRC_TYPE='S' and CSRC_PROD=@prodid and CSRC_PRICE=@price and CSRC_SAL_ATTR='A' and CSRC_ATTR_YM=@AccYM and CSRC_QTY+@qty=0)
                                            delete from NV_CUST_REC where CSRC_YM=@AccYM and CSRC_NO=@NewNumber and CSRC_TYPE='S' and CSRC_PROD=@prodid and CSRC_PRICE=@price and CSRC_SAL_ATTR='A' and CSRC_ATTR_YM=@AccYM
                                                else if exists(select * from NV_CUST_REC where CSRC_YM=@AccYM and CSRC_NO=@NewNumber and CSRC_TYPE='S' and CSRC_PROD=@prodid and CSRC_PRICE=@price and CSRC_SAL_ATTR='A' and CSRC_ATTR_YM=@AccYM and CSRC_QTY+@qty<>0)
                                            update NV_CUST_REC set CSRC_CUST_ID=@CustID, CSRC_DATE=convert(varchar(8),getDate(),112), CSRC_QTY=CSRC_QTY+@qty where CSRC_YM=@AccYM and CSRC_NO=@NewNumber and CSRC_TYPE='S' and CSRC_PROD=@prodid and CSRC_PRICE=@price and CSRC_SAL_ATTR='A' and CSRC_ATTR_YM=@AccYM
                                                else
                                            insert into NV_CUST_REC(CSRC_YM,CSRC_NO,CSRC_TYPE,CSRC_PROD,CSRC_PRICE,CSRC_SAL_ATTR,CSRC_ATTR_YM,CSRC_CUST_ID,CSRC_DATE,CSRC_QTY)
                                            values(@AccYM,@NewNumber,'S',@prodid,@price,'A',@AccYM,@CustID,convert(varchar(8),getDate(),112),@qty)";
                string strSql_Cust_Trans = @"if exists(select * from NV_CUST_TRANS where CSTN_YM=@AccYM and CSTN_CUST_ID=@CustID and CSTN_PROD_ID=@prodid and CSTN_ATTR_YM=@AccYM and CSTN_PRICE=@price and CSTN_SAL_ATTR='A')
                                            update NV_CUST_TRANS set CSTN_SAL_QTY=CSTN_SAL_QTY+@qty, CSTN_END_QTY=CSTN_END_QTY+@qty where CSTN_YM=@AccYM and CSTN_CUST_ID=@CustID and CSTN_PROD_ID=@prodid and CSTN_ATTR_YM=@AccYM and CSTN_PRICE=@price and CSTN_SAL_ATTR='A'
                                               else
                                            insert into NV_CUST_TRANS(CSTN_YM,CSTN_CUST_ID,CSTN_PROD_ID,CSTN_ATTR_YM,CSTN_PRICE,CSTN_SAL_ATTR,CSTN_BGN_QTY,CSTN_SAL_QTY,CSTN_RTN_QTY,CSTN_ROT_I_QTY,CSTN_ROT_O_QTY,CSTN_SETTLE_QTY,CSTN_END_QTY)
                                            values (@AccYM,@CustID,@prodid,@AccYM,@price,'A',0,@qty,0,0,0,0,@qty)

                                            update NV_CUST_TRANS set CSTN_BGN_QTY=CSTN_BGN_QTY+@qty, CSTN_END_QTY=CSTN_END_QTY+@qty where CSTN_CUST_ID=@CustID and CSTN_PROD_ID=@prodid and CSTN_ATTR_YM=@AccYM and CSTN_PRICE=@price and CSTN_SAL_ATTR='A' and CSTN_YM>@AccYM";


                DataTable dtSubjoin = wmp.Module.GetValue<DataTable>("dtSubjoin");
                dbap.SQL = @"select case when max(SALM_NO) IS NULL then  'S'+substring(convert(varchar(8),getDate(),112),3,6)+'001' 
                            else 'S'+substring(convert(varchar(8),getDate(),112),3,6)+right('000'+convert(varchar(4),convert(int,right(max(SALM_NO),3))+1),3) end 
                            from NV_SALE_MST
                            where SALM_NO like 'S'+substring(convert(varchar(8),getDate(),112),3,6)+'%'";
                string strNewNumber = Dao.Query(dbap).Tables[0].Rows[0][0].ToString();


                for (int w = 0; w < dtSubjoin.Rows.Count; w++)
                {
                    dbap.SQL_Parameters.Clear();
                    dbap.SQL_Parameters.Add("NewNumber", strNewNumber);
                    dbap.SQL_Parameters.Add("prodid", dtSubjoin.Rows[w]["BAKP_PROD"].ToString());
                    dbap.SQL_Parameters.Add("qty", dtSubjoin.Rows[w]["BAKP_QTY"].ToString());
                    dbap.SQL_Parameters.Add("price", dtSubjoin.Rows[w]["BAKP_PRICE"].ToString());
                    dbap.SQL_Parameters.Add("seq", (w + 1));
                    dbap.SQL_Parameters.Add("CustID", wmp.GetValue<string>("CustID"));
                    dbap.SQL_Parameters.Add("Number", dtSubjoin.Rows[w]["BAKP_NO"].ToString());
                    dbap.SQL_Parameters.Add("Stock", strStock);
                    dbap.SQL_Parameters.Add("AccYM", strYear);


                    dbap.SQL = strNV_BAKD_PROD_Update;
                    Dao.ExecuteNoQuery(dbap);

                    dbap.SQL = strNV_Prod_Stock_Update;
                    Dao.ExecuteNoQuery(dbap);
                    GlobalCommon.Logger.WriteLog(WITS.Framework.Interface.LoggerLevel.DEBUG, dbap.SQL + "(5)" + "退貨單主檔表退貨單號:" + strOdd);

                    dbap.SQL = strNV_Prod_Trans_Update;
                    Dao.ExecuteNoQuery(dbap);
                    GlobalCommon.Logger.WriteLog(WITS.Framework.Interface.LoggerLevel.DEBUG, dbap.SQL + "(6)" + "退貨單主檔表退貨單號:" + strOdd);



                    if (Convert.ToInt32(dtSubjoin.Rows[w]["BAKP_QTY"].ToString()) != 0)
                    {
                        //modified by wenjingcheng  for jira-242 begin 2014-4-3
                        dbap.SQL = strNV_Sale_Dtl_Insert;
                        Dao.ExecuteNoQuery(dbap);
                        //modified by wenjingcheng  for jira-242 end 2014-4-3
                        //modified by wenjingcheng  for jira-257 begin 2014-4-19
                        dbap.SQL = strNV_Prod_Rec_Update;
                        Dao.ExecuteNoQuery(dbap);
                        GlobalCommon.Logger.WriteLog(WITS.Framework.Interface.LoggerLevel.DEBUG, dbap.SQL + "(7)" + "退貨單主檔表退貨單號:" + strOdd);
                        //modified by wenjingcheng  for jira-257 end 2014-4-19
                        dbap.SQL = strSql_Cust_Stock;
                        Dao.ExecuteNoQuery(dbap);
                        dbap.SQL = strSql_Cust_Rec;
                        Dao.ExecuteNoQuery(dbap);
                        dbap.SQL = strSql_Cust_Trans;
                        Dao.ExecuteNoQuery(dbap);
                    }

                    Price_QTY += Convert.ToInt32(dtSubjoin.Rows[w]["BAKP_QTY"].ToString());
                }

                if (Price_QTY != 0)
                {
                    dbap.SQL_Parameters.Clear();
                    dbap.SQL_Parameters.Add("NewNumber", strNewNumber);
                    dbap.SQL_Parameters.Add("Stock", strStock);
                    dbap.SQL_Parameters.Add("AccYM", strYear);
                    dbap.SQL_Parameters.Add("CustID", wmp.GetValue<string>("CustID"));
                    dbap.SQL_Parameters.Add("ADD_USER_ID", wmp.Module.GetValue<string>("ADD_USER_ID"));
                    dbap.SQL_Parameters.Add("ADD_DATE", wmp.Module.GetValue<string>("ADD_DATE"));
                    dbap.SQL_Parameters.Add("ADD_TIME", wmp.Module.GetValue<string>("ADD_TIME"));
                    dbap.SQL = strNV_Sale_Mst_Insert;
                    Dao.ExecuteNoQuery(dbap);

                }
                //end of add

                Dao.Commit();
                Dao.Close();
            }
            return true;
        }
    }
}