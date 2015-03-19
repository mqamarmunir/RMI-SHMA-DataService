using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Data.SqlClient;

namespace RMI_DataService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class Service1 : IRMIDataService
    {
        #region OPERATION CONTRACT

        public List<Details> GetReceivableData(string flag,string AorP,string fromDate,string toDate)
        {
            toDate = toDate.Substring(0, 2) + "/" + toDate.Substring(2, 2) + "/" + toDate.Substring(4, 4);
            fromDate = fromDate.Substring(0, 2) + "/" + fromDate.Substring(2, 2) + "/" + fromDate.Substring(4, 4);
            List<Details> objLabDetails = new List<Details>();

             if (flag.ToUpper() == "L")
             {
                 return GetPathology(fromDate, toDate, AorP);
             }
             else if (flag.ToUpper() == "C")
             {
                 return GetCardiology(fromDate, toDate, AorP);
             }
             else if (flag.ToUpper() == "R")
             {
                 return GetRadiology(fromDate, toDate, AorP);
             }
             else if (flag.ToUpper() == "P")
             {
                 return GetOPD(fromDate, toDate,AorP);
             }
             else if (flag.ToUpper() == "G")
             {
                  return GetGeneral(fromDate, toDate,AorP);
             }
             else if (flag.ToUpper() =="PH")
             {
                 return GetPharmacy(fromDate,toDate,AorP);
                 
             }
             else if (flag.ToUpper() == "IPD")
             {
                 return GetIPD(fromDate, toDate,AorP);
             }
             else if (flag.ToUpper() == "A")
	         {
		          return GetAllReceivables(fromDate,toDate,AorP);
	         }
             else
             {
                 return objLabDetails;
             }
        }
        
        public List<PayableDetails> GetPayableDataRefund(string flag, string AorP, string fromDate, string toDate) 
        {
            string Query = "";
            toDate = toDate.Substring(0, 2) + "/" + toDate.Substring(2, 2) + "/" + toDate.Substring(4, 4);
            fromDate = fromDate.Substring(0, 2) + "/" + fromDate.Substring(2, 2) + "/" + fromDate.Substring(4, 4);

            List<PayableDetails> objDetails = new List<PayableDetails>();

            if (flag.ToUpper() == "IPD")
            {
                return GetIPDRefund(fromDate, toDate, AorP);

            }
            else if (flag.ToUpper() == "L")
            {
                return GetLabRefund(fromDate, toDate, AorP);
            }
            else if (flag.ToUpper() == "C")
            {
                return CardiologyRefund(fromDate, toDate, AorP);
            }
            else if (flag.ToUpper() == "R")
            {
                return GetRadiologyRefund(fromDate, toDate, AorP);


            }
            else if (flag.ToUpper() == "P")
            {
                return GetOPDRefund(fromDate, toDate, AorP);


            }
            else if (flag.ToUpper() == "G")
            {

                return GetGeneralRefund(fromDate, toDate, AorP);

            }
            else if (flag.ToUpper() == "PH")
            {
                return GetPharmacyRefund(fromDate, toDate, AorP);
                //return getPhRefund(fromDate, toDate, AorP);
            }
            else if (flag.ToUpper() == "A")
            {
                return GetAllRefund(fromDate, toDate, AorP);
            }
            else
                return null;
            
        }
        
        private List<PayableDetails> GetAllRefund(string fromDate, string toDate, string AorP)
        {
            List<PayableDetails> _objDetails = new List<PayableDetails>();
            _objDetails.AddRange(GetLabRefund(fromDate,toDate,AorP));
            _objDetails.AddRange(CardiologyRefund(fromDate,toDate,AorP));
            _objDetails.AddRange(GetRadiologyRefund(fromDate,toDate,AorP));
            _objDetails.AddRange(GetOPDRefund(fromDate, toDate, AorP));
            _objDetails.AddRange(GetGeneralRefund(fromDate, toDate, AorP));
            _objDetails.AddRange(GetPharmacyRefund(fromDate, toDate, AorP));
            _objDetails.AddRange(GetIPDRefund(fromDate, toDate, AorP));

            return _objDetails;


        }
        
        public List<ConsumptionDetails> GetConsumption(string flag, string fromDate, string toDate)
        {
            string Query = "";
            
            List<ConsumptionDetails> objDetails = new List<ConsumptionDetails>();

            toDate = toDate.Substring(0, 2) + "/" + toDate.Substring(2, 2) + "/" + toDate.Substring(4, 4);
            fromDate = fromDate.Substring(0, 2) + "/" + fromDate.Substring(2, 2) + "/" + fromDate.Substring(4, 4);

            if (flag.ToUpper() == "C")
            {
                Query = @"select c.admissionno as admno,
                           c.testid as service_detail,
                           c.Charges,
                           '3-01-01-06-001-001-1' as expense_head,
                           'inpatient-cardiology' as costhead
                            from ca_vwaitingqueue c
                            where c.admissionno is not null
                            and c.status <> 'X'
                            and to_date(to_char(c.enteredon, 'DD/MM/YYYY'), 'DD/MM/YYYY') between
                           to_date('" + fromDate + @"', 'DD/MM/YYYY') and
                           to_date('" + toDate + "', 'DD/MM/YYYY')";
            }
            else if (flag.ToUpper() == "R")
            {
                Query = @"select r.admno,
                                
                                r.invid as service_detail,
                                r.TestAmount as charges,
                                '3-01-01-07-001-001-1' as expense_head,
                                'inpatient-radiology' as costhead
                        from rd_vrdconsumptionreport r
                        where r.admno is not null
                        and to_date(to_char(r.testdate, 'DD/MM/YYYY'), 'DD/MM/YYYY') between
                        to_date('"+fromDate+@"', 'DD/MM/YYYY') and
                        to_date('"+toDate+"', 'DD/MM/YYYY') ";
            }
            else if (flag.ToUpper() == "L")
            {
                Query = @"select v.PADMNo as admno,
                                    v.Testid as service_detail,
                                    v.Charges,
                                    '3-01-01-04-001-001-1' as expense_head,
                                    'inpatient-laboratory' as costhead
                                from whims.wd_vpatientconsumption v
                                where

                                to_date(to_char(v.entrydatetime, 'DD/MM/YYYY'), 'DD/MM/YYYY') between
                                to_date('" + fromDate + @"', 'DD/MM/YYYY') and
                                to_date('" + toDate + "', 'DD/MM/YYYY')";
            }
            else if (flag.ToUpper() == "V")
            {
                return VisitsConsumption(fromDate, toDate);
            }
            else if (flag.ToUpper() == "WFS")
            {
                return WardandFloorStockConsumption(fromDate, toDate);
            }
            else if (flag.ToUpper() == "CATH")
            {
                return CathConsumption(fromDate, toDate);
            }
            else if (flag.ToUpper() == "OT")
            {
                return GetOTPharmacyConsumption(fromDate, toDate);
                
            }
            
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
            OleDbConnection oraConnection = new OleDbConnection(connectionString);
            oraConnection.Open();
            OleDbDataAdapter oraAdapter = new OleDbDataAdapter(Query, oraConnection);
            oraConnection.Close();
            DataTable dt = new DataTable();
            oraAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
               for (int i = 0; i < dt.Rows.Count; i++)
			   {
                   ConsumptionDetails details = new ConsumptionDetails();
                   details.AdmissionNo = dt.Rows[i][0].ToString();
                   details.ServiceDetails = dt.Rows[i][1].ToString();
                   details.Charges = dt.Rows[i][2].ToString();
                   details.ExpanseHead = dt.Rows[i][3].ToString();
                   details.CostHead = dt.Rows[i][4].ToString();
                   objDetails.Add(details);
			   }
            }

            return objDetails;
           

        }

        public List<OTConsumptionDetails> GetOTProcedureConsumption(string fromDate, string toDate)
        {
            toDate = toDate.Substring(0, 2) + "/" + toDate.Substring(2, 2) + "/" + toDate.Substring(4, 4);
            fromDate = fromDate.Substring(0, 2) + "/" + fromDate.Substring(2, 2) + "/" + fromDate.Substring(4, 4);

            List<OTConsumptionDetails> objDetails = new List<OTConsumptionDetails>();
            string Query = @"  select ot.admno,  d.personshare, d.hospitalshare,d.revcode as expense_head,c.description as costhead,con.personid
                              from ot_otbooking       ot,
                               
                                   ot_otbookingdetail d
                                   left join hr_tconsultant con on con.revcode = d.revcode,
                                   hr_tpersonnel      p,
                                   wd_vadmpackage     w,
                                   gl_tcoa c
                             where ot.status = 'D'
                               and d.personid = p.personid
                               and c.accountno = d.revcode
                               and d.otbookingid = ot.otbookingid
                               and w.Admno = d.admno
                               and d.packageid = w.packageid
                            and to_date(to_char(ot.otdate,'dd/MM/yyyy'),'dd/MM/yyyy') between to_date('" + fromDate + @"','dd/MM/yyyy') and 
                            to_date('" + toDate + "','dd/MM/ yyyy') order by w.amount desc, d.headingid";


            string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
            OleDbConnection oraConnection = new OleDbConnection(connectionString);
            oraConnection.Open();
            OleDbDataAdapter oraAdapter = new OleDbDataAdapter(Query, oraConnection);
            oraConnection.Close();
            DataTable dt = new DataTable();
            oraAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    OTConsumptionDetails details = new OTConsumptionDetails();
                    details.AdmissionNo = dt.Rows[i][0].ToString();
                    details.PersonShare = dt.Rows[i][1].ToString();
                    details.HospitalShare = dt.Rows[i][2].ToString();
                    details.ExpenseHead = dt.Rows[i][3].ToString();
                    details.CostHead = dt.Rows[i][4].ToString();
                    if (string.IsNullOrEmpty(dt.Rows[i][5].ToString()))
                    {
                        details.ConsAff = null;
                    }
                    else
                    {
                        details.ConsAff = dt.Rows[i][16].ToString();
                    }
                    objDetails.Add(details);
                }
            }

            return objDetails;
            
        }

        public List<ConsumptionDetails> GetWardAndFloorStockReturn(string fromDate, string toDate)
        {
            toDate = toDate.Substring(0, 2) + "/" + toDate.Substring(2, 2) + "/" + toDate.Substring(4, 4);
            fromDate = fromDate.Substring(0, 2) + "/" + fromDate.Substring(2, 2) + "/" + fromDate.Substring(4, 4);

            List<ConsumptionDetails> _objDetails = new List<ConsumptionDetails>();
            string query = @"Select rm.ADMISSIONNO as admno,
                                   
                                    m.medicineid as service_detail,
                                   rd.TOTALAMOUNT as charges,
                                   rm.service_charge
       
                                   as service_charges,
                                   s.REVENUECODE as expense_head,
                                   c.description as costhead,
                                con.personid
                              from ph_Tpatient_requestm rm
                              join ph_Tpatient_requestd rd on rm.requestid = rd.requestid
                              join ph_tmed_master m on m.medicineid = rd.medicineid
                              join PH_TUNIT u on m.UNITID = u.UNITID
                              join PH_TNATURE n on n.NATUREID = m.NATUREID
                              join PH_TPatient_Return r on r.Rx_No = rd.RXNO
                                                       and r.StatusId = 'A'
                              join HR_TSUBDEPARTMENT s on s.SUBDEPARTMENTID = rm.SUBDEPTID
                                left join hr_tconsultant con on con.revcode = s.revenuecode
                              left outer join GL_TCOA c on c.accountno = s.REVENUECODE
                             where 1 = 1
                               and rd.qty_issued is not null
                               and rm.status = 'I'
                               and rm.subdeptid in ('0164', '0316')
                               and convert(date, r.enteredon, 103) between
                                   convert(date, '" + fromDate + @"', 103) and
                                   convert(date, '" + toDate + @"', 103)
                               and rm.ADMISSIONNO is not null";
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["sql"].ToString();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter dAdapter = new SqlDataAdapter(query, connectionString);
            connection.Close();
            DataTable dt = new DataTable();
            dAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ConsumptionDetails details = new ConsumptionDetails();
                    details.AdmissionNo = dt.Rows[i][0].ToString();
                    details.ServiceDetails = dt.Rows[i][1].ToString();
                    details.Charges = dt.Rows[i][2].ToString();
                    details.ExpanseHead = dt.Rows[i][3].ToString();
                    details.CostHead = dt.Rows[i][4].ToString();
                    if (string.IsNullOrEmpty(dt.Rows[i][5].ToString()))
                    {
                        details.ConsAff = null;
                    }
                    else
                    {
                        details.ConsAff = dt.Rows[i][16].ToString();
                    }
                    _objDetails.Add(details);
                }
            }
            _objDetails.AddRange(GetFloorStockReturn(fromDate, toDate));

            return _objDetails;

        }
        
        #region Update

        public int UpdateAC(string referenceNo, string importNo)
        {
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
            OleDbConnection conn = new OleDbConnection(connectionString);
            string query = "update ac_cashrechistory set R_U = '" + importNo + "' where rno ='" + referenceNo + "'";
            conn.Open();
            OleDbCommand Command = new OleDbCommand(query, conn);
            int cc = Command.ExecuteNonQuery();
            conn.Close();
            return cc;
        }

        public int UpdateBL(string referenceNo, string importNo)
        {
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
            OleDbConnection conn = new OleDbConnection(connectionString);
            string query = "update bl_tbilldetail set R_U = '" + importNo + "' where paidno='" + referenceNo + "'";
            conn.Open();
            OleDbCommand Command = new OleDbCommand(query, conn);
            int cc = Command.ExecuteNonQuery();
            conn.Close();
            return cc;
        }

        #endregion

        #endregion

        #region PRIVATE METHODS

        #region Private Methods For Refund

        private List<PayableDetails> GetIPDRefund(string fromDate, string toDate, string AorP)
        {
            //List<PayableDetails> objDetails = new List<PayableDetails>();
            string Query = @"select 'In-Patient' as dept,
                                    p.PRNO as prno,
                                    t.packageid as service_detail, 
                                    t.revcode as expense_head,
                                    e.CompanyName as panel,
                                    d.paymentmade as price,
                                    d.paymentmade as cost,
                                    0 as discount,
                                    0 as sales_tax,
                                    nvl(d.staff_discount,0) as insurance_amount,
                                    g.description as cost_head,
                                    d.paidno as referenceno,
                                    u.name as refundtype,
                                    vp.PersonName as enteredby,
                                    d.enteredon as enteredon,
                                    t.departmentid
                                    ,con.personid
      
                                    from whims2.Bl_Tbillmaster m
                                    inner join whims2.bl_tbilldetail d on m.billid = d.billid
                                    inner join whims2.Hr_Tpackagemaster t on t.packageid = d.packageid
                                    left join whims2.hr_tconsultant con on con.revcode = t.revcode
                                    inner join whims2.pr_vpatientreg p on p.PRNO = m.prno
                                    inner join whims2.Gl_Tcoa g on g.accountno=t.revcode
                                    inner join whims2.Bl_Tbillrefund f on f.billid=m.billid
                                    inner join whims2.hr_vpersonnel vp on vp.PERSONID = d.enteredby
                                    inner join whims2.hr_trefundtype u on u.refundtypeid = f.refundtypeid
                                    left outer join whims2.or_vemployee e on e.EMPLOYEEID=p.EMPLOYEEID
                                    where to_date(to_char(d.enteredon, 'dd/MM/yyyy'), 'dd/MM/yyyy') between
                                    to_date('" + fromDate + @"', 'dd/MM/yyyy') and
                                    to_date('" + toDate + @"', 'dd/MM/yyyy')
                                    and d.paymentmade <0";
            if (AorP.ToUpper() == "P")
            {
                Query += " and d.voucherno is null ";
            }
            Query += " order by m.billid ";

            List<PayableDetails> objDetails = new List<PayableDetails>();
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
            OleDbConnection oraConnection = new OleDbConnection(connectionString);
            oraConnection.Open();
            OleDbDataAdapter oraAdapter = new OleDbDataAdapter(Query, oraConnection);
            oraConnection.Close();
            DataTable dt = new DataTable();
            oraAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PayableDetails details = new PayableDetails();
                    details.Department = dt.Rows[i][0].ToString();
                    details.PR_No = dt.Rows[i][1].ToString();
                    details.ServiceDetails = dt.Rows[i][2].ToString();
                    details.ExpanseHead = dt.Rows[i][3].ToString();
                    details.Panel = dt.Rows[i][4].ToString();
                    if (Convert.ToInt32(dt.Rows[i][5].ToString()) > 0)
                    {
                        details.Price = dt.Rows[i][5].ToString();
                        details.Cost = dt.Rows[i][6].ToString();
                    }
                    else
                    {
                        details.Price = (Convert.ToInt32(dt.Rows[i][5].ToString()) * Convert.ToInt32("-1")).ToString();
                        details.Cost = (Convert.ToInt32(dt.Rows[i][6].ToString()) * Convert.ToInt32("-1")).ToString();
                    }

                    details.Discount = dt.Rows[i][7].ToString();
                    details.Sales_Tax = dt.Rows[i][8].ToString();
                    details.Insurance_Amount = dt.Rows[i][9].ToString();
                    details.CostHead = dt.Rows[i][10].ToString();
                    details.ReferenceNo = dt.Rows[i][11].ToString();
                    details.RefundType = "R";
                    details.EnteredBy = dt.Rows[i][13].ToString();
                    details.EnteredOn = dt.Rows[i][14].ToString();
                    details.SubdepartmentId = dt.Rows[i][15].ToString();
                    if (string.IsNullOrEmpty(dt.Rows[i][16].ToString()))
                    {
                        details.ConsAff = null;  
                    }
                    else
                    {
                        details.ConsAff = dt.Rows[i][16].ToString();
                    }

                    objDetails.Add(details);
                }

            }
            return objDetails;
        }

        private List<PayableDetails> GetLabRefund(string fromDate, string toDate, string AorP)
        {
            List<PayableDetails> objDetails = new List<PayableDetails>();
            string Query = @"select 'Pathology' as dept,
                                   p.PRNO as prno,
                                   
                                    t.testid as service_detail,
                                   '3-01-01-04-002-001-1' as expense_head,
                                   e.CompanyName as panel,
                                   d.charges as price,
                                   d.charges as cost,
                                   c.discount,
                                   0 as sales_tax,
                                   nvl(c.staff_discount, 0) as insurance_amount,
                                   'outpatient-laboratory' as cost_head,
                                   f.refundno as referenceno,
                                   u.name as refundtype,
                                   vp.PersonName as enteredby,
                                   m.entrydatetime as enteredon,
                                   s.sectionid,s.sectionname
                             
                             from whims.Ls_Tmtransaction m
                             inner join whims.ls_tdtransaction d on m.mserialno = d.mserialno
                             inner join whims.ls_ttest t on t.testid = d.testid
                             inner join whims.ls_tsection s on s.sectionid = t.sectionid
                             inner join whims2.pr_vpatientreg p on trim(p.PRNO) = trim(m.prno)
                             inner join whims2.ac_tcashreceived c on c.paidno = m.paidno
                                                                 and trim(c.referenceno) =
                                                                     trim(m.labid)
                             inner join whims2.ac_tcashrefund f on f.paidno = c.paidno
                             inner join whims2.hr_trefundtype u on u.refundtypeid = f.refundtypeid
                             inner join whims2.hr_vpersonnel vp on vp.PERSONID = f.cashierid
                              left outer join whims2.or_vemployee e on e.EMPLOYEEID = p.EMPLOYEEID
                             inner join whims2.ac_cashrechistory h on h.rno = f.refundno
                                                                  and h.amount < 0
                                                                  and h.prno = p.PRNO
                             inner join hr_tsubdepartment sd on vp.SUBDEPARTMENTID = sd.subdepartmentid
                             where to_date(to_char(h.edate, 'dd/MM/yyyy'), 'dd/MM/yyyy') between
                                   to_date('" + fromDate + @"', 'dd/MM/yyyy') and
                                to_date('" + toDate + @"', 'dd/MM/yyyy')
                               and m.iop = 'O'";
            if (AorP.ToUpper() == "P")
            {
                Query += " and h.voucherno is null ";
            }
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
            OleDbConnection oraConnection = new OleDbConnection(connectionString);
            oraConnection.Open();
            OleDbDataAdapter oraAdapter = new OleDbDataAdapter(Query, oraConnection);
            oraConnection.Close();
            DataTable dt = new DataTable();
            oraAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PayableDetails details = new PayableDetails();
                    details.Department = dt.Rows[i][0].ToString();
                    details.PR_No = dt.Rows[i][1].ToString();
                    details.ServiceDetails = dt.Rows[i][2].ToString();
                    details.ExpanseHead = dt.Rows[i][3].ToString();
                    details.Panel = dt.Rows[i][4].ToString();
                    details.Price = dt.Rows[i][5].ToString();
                    details.Cost = dt.Rows[i][6].ToString();
                    details.Discount = dt.Rows[i][7].ToString();
                    details.Sales_Tax = dt.Rows[i][8].ToString();
                    details.Insurance_Amount = dt.Rows[i][9].ToString();
                    details.CostHead = dt.Rows[i][10].ToString();
                    details.ReferenceNo = dt.Rows[i][11].ToString();
                    details.RefundType = "R";
                    details.EnteredBy = dt.Rows[i][13].ToString();
                    details.EnteredOn = dt.Rows[i][14].ToString();
                    details.SubdepartmentId = dt.Rows[i][15].ToString();

                    objDetails.Add(details);
                }

            }
            return objDetails;
        }

        private List<PayableDetails> CardiologyRefund(string fromDate, string toDate, string AorP)
        {
            string Query = @"select 'Cardiology' as dept,
                                p.PRNO as prno,
                                
                                t.testid as service_detail,
                                s.revenuecode as expense_head,
                                e.CompanyName as panel,
                                t.norcharges as price,
                                t.norcharges as cost,
                                c.discount,
                                0 as sales_tax,
                                nvl(c.staff_discount,0) as insurance_amount,
                                g.description as cost_head,
                                f.refundno as referenceno  ,
                                u.name as refundtype,
                                vp.PersonName as enteredby,
                                f.refunddatetime as enteredOn,
                                t.subdepartmentid
                                ,con.personid    
      
                                from whims2.ca_trequestmaster m
                                inner join whims2.ca_trequestdetail d on m.requestid = d.requestid
                                inner join whims2.Ca_Ttestregistration t on t.testid = d.testid
                                inner join whims2.pr_vpatientreg p on p.PRNO = m.prno
                                inner join whims2.pr_tpatientvisitd vd on trim(vd.visitno)=trim(m.visitno) and p.prno=vd.prno
                                inner join whims2.ac_tcashreceived c on  trim(m.requestno) = trim(c.referenceno)
                                inner join whims2.ac_tcashrefund f on f.paidno=c.paidno
                                inner join whims2.hr_trefundtype u on u.refundtypeid = f.refundtypeid
                                inner join whims2.hr_vpersonnel vp on vp.PERSONID = f.requestby
                                left outer join whims2.or_vemployee e on e.EMPLOYEEID=p.EMPLOYEEID
                                inner join whims2.ac_cashrechistory  h on trim(h.rno)=trim(f.refundno) and h.amount<0
                                and h.prno = p.PRNO
                                inner join Hr_Tsubdepartment s on s.subdepartmentid=t.subdepartmentid
                                left join whims2.hr_tconsultant con on con.revcode = s.revenuecode
                                inner join Gl_Tcoa g on g.accountno = s.revenuecode
                                
                                where to_date(to_char(h.edate, 'dd/MM/yyyy'), 'dd/MM/yyyy') between
                                to_date('" + fromDate + @"', 'dd/MM/yyyy') and
                                to_date('" + toDate + @"', 'dd/MM/yyyy')
                                and m.ptype = 'O' and m.paid='Y'";
            if (AorP.ToUpper() == "P")
            {
                Query += " and h.voucherno is null ";
            }
            Query += " order by m.requestid";

            List<PayableDetails> objDetails = new List<PayableDetails>();
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
            OleDbConnection oraConnection = new OleDbConnection(connectionString);
            oraConnection.Open();
            OleDbDataAdapter oraAdapter = new OleDbDataAdapter(Query, oraConnection);
            oraConnection.Close();
            DataTable dt = new DataTable();
            oraAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PayableDetails details = new PayableDetails();
                    details.Department = dt.Rows[i][0].ToString();
                    details.PR_No = dt.Rows[i][1].ToString();
                    details.ServiceDetails = dt.Rows[i][2].ToString();
                    details.ExpanseHead = dt.Rows[i][3].ToString();
                    details.Panel = dt.Rows[i][4].ToString();
                    details.Price = dt.Rows[i][5].ToString();
                    details.Cost = dt.Rows[i][6].ToString();
                    details.Discount = dt.Rows[i][7].ToString();
                    details.Sales_Tax = dt.Rows[i][8].ToString();
                    details.Insurance_Amount = dt.Rows[i][9].ToString();
                    details.CostHead = dt.Rows[i][10].ToString();
                    details.ReferenceNo = dt.Rows[i][11].ToString();
                    details.RefundType = "R";
                    details.EnteredBy = dt.Rows[i][13].ToString();
                    details.EnteredOn = dt.Rows[i][14].ToString();
                    details.SubdepartmentId = dt.Rows[i][15].ToString();
                    if (string.IsNullOrEmpty(dt.Rows[i][16].ToString()))
                    {
                        details.ConsAff = null;
                    }
                    else
                    {
                        details.ConsAff = dt.Rows[i][16].ToString();
                    }

                    objDetails.Add(details);
                }

            }
            return objDetails;
        }

        private List<PayableDetails> GetRadiologyRefund(string fromDate, string toDate, string AorP)
        {
            string Query = @"select 'Radiology' as dept,
                                p.PRNO as prno,
                                
                                t.invid as service_detail,
                                s.revenuecode as expense_head,
                                e.CompanyName as panel,
                                t.normalcharges as price,
                                t.normalcharges as cost,
                                c.discount,
                                0 as sales_tax,
                                nvl(c.staff_discount,0) as insurance_amount,
                                g.description as cost_head,
                                f.refundno as referenceno   ,
                                u.name as refundtype,
                                vp.PersonName as enteredby,
                                f.refunddatetime as enteredOn,
                                t.subdepartmentid
                                ,con.personid    
                                
                                from whims2.Rd_Ttransmaster m
                                inner join whims2.rd_ttransdetail d on m.transid = d.transid
                                inner join whims2.rd_tinvestigation t on t.invid = d.invid
                                inner join whims2.pr_vpatientreg p on p.PRNO = m.prno
                                inner join whims2.pr_tpatientvisitd vd on trim(vd.visitno)=trim(m.visitno) and p.prno=vd.prno
                                inner join whims2.ac_tcashreceived c on  trim(m.radiologyno) = trim(c.referenceno)
                                inner join whims2.ac_tcashrefund f on f.paidno = c.paidno
                                inner join whims2.hr_trefundtype u on u.refundtypeid=f.refundtypeid
                                left outer join whims2.or_vemployee e on e.EMPLOYEEID=p.EMPLOYEEID
                                inner join whims2.ac_cashrechistory  h on trim(h.rno)=trim(f.refundno) and h.amount<0
                                and h.prno = p.PRNO
                                inner join Hr_Tsubdepartment s on s.subdepartmentid=t.subdepartmentid
                                 left join whims2.hr_tconsultant con on con.revcode = s.revenuecode
                                inner join Gl_Tcoa g on g.accountno = s.revenuecode
                                inner join whims2.hr_vpersonnel vp on vp.PERSONID = f.cashierid

                                where to_date(to_char(h.edate, 'dd/MM/yyyy'), 'dd/MM/yyyy') between
                                to_date('" + fromDate + @"', 'dd/MM/yyyy') and
                                to_date('" + toDate + @"', 'dd/MM/yyyy')
                                and m.patienttype = 'E' and m.paid='Y'";
            if (AorP.ToUpper() == "P")
            {
                Query += " and h.voucherno is null ";
            }
            Query += " order by m.transid";

            List<PayableDetails> objDetails = new List<PayableDetails>();
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
            OleDbConnection oraConnection = new OleDbConnection(connectionString);
            oraConnection.Open();
            OleDbDataAdapter oraAdapter = new OleDbDataAdapter(Query, oraConnection);
            oraConnection.Close();
            DataTable dt = new DataTable();
            oraAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PayableDetails details = new PayableDetails();
                    details.Department = dt.Rows[i][0].ToString();
                    details.PR_No = dt.Rows[i][1].ToString();
                    details.ServiceDetails = dt.Rows[i][2].ToString();
                    details.ExpanseHead = dt.Rows[i][3].ToString();
                    details.Panel = dt.Rows[i][4].ToString();
                    details.Price = dt.Rows[i][5].ToString();
                    details.Cost = dt.Rows[i][6].ToString();
                    details.Discount = dt.Rows[i][7].ToString();
                    details.Sales_Tax = dt.Rows[i][8].ToString();
                    details.Insurance_Amount = dt.Rows[i][9].ToString();
                    details.CostHead = dt.Rows[i][10].ToString();
                    details.ReferenceNo = dt.Rows[i][11].ToString();
                    details.RefundType = "R";
                    details.EnteredBy = dt.Rows[i][13].ToString();
                    details.EnteredOn = dt.Rows[i][14].ToString();
                    details.SubdepartmentId = dt.Rows[i][15].ToString();
                    if (string.IsNullOrEmpty(dt.Rows[i][16].ToString()))
                    {
                        details.ConsAff = null;
                    }
                    else
                    {
                        details.ConsAff = dt.Rows[i][16].ToString();
                    }

                    objDetails.Add(details);
                }

            }
            return objDetails;
        }

        private List<PayableDetails> GetOPDRefund(string fromDate, string toDate, string AorP)
        {
            string Query = @"select distinct 'OPD' as dept,
                            p.PRNO as prno,
                            t.serviceid as service_detail,
                            s.revenuecode as expense_head,
                            e.CompanyName as panel,
                            t.rate as price,
                            t.rate as cost,
                            c.discount,
                            0 as sales_tax,
                            nvl(c.staff_discount,0) as insurance_amount,
                            g.description as cost_head,
                            f.refundno as referenceno,
                            u.name as refuntype,
                            vp.PersonName as enteredby,
                            f.refunddatetime as enteredOn,
                            t.subdepartmentid
                            ,s.personid

                            from whims2.pr_tpatientvisitm m
                            inner join whims2.pr_tpatientvisitd vd on trim(vd.visitno)=trim(m.visitno)
                            inner join whims2.hr_Tservice t on t.serviceid = vd.serviceid
                            inner join whims2.pr_vpatientreg p on p.PRNO = m.prno
                            inner join whims2.ac_tcashreceived c on  trim(m.visitno) = trim(c.referenceno) and vd.paidno=c.paidno
                            and c.type='P'
                            inner join whims2.ac_tcashrefund f on f.paidno=c.paidno
                            inner join whims2.Hr_Trefundtype u on u.refundtypeid=f.refundtypeid
                            left outer join whims2.or_vemployee e on e.EMPLOYEEID=p.EMPLOYEEID
                            inner join whims2.ac_cashrechistory  h on trim(h.rno)=trim(f.refundno) and h.amount<0
                            and h.prno = p.PRNO
                            inner join Hr_Tsubdepartment s on s.subdepartmentid=t.subdepartmentid
                           
                          
                            inner join Gl_Tcoa g on g.accountno = s.revenuecode
                            inner join whims2.hr_vpersonnel vp on vp.PERSONID = f.cashierid
                                   
                            where to_date(to_char(h.edate, 'dd/MM/yyyy'), 'dd/MM/yyyy') between
                            to_date('" + fromDate + @"', 'dd/MM/yyyy') and
                             to_date('" + toDate + @"', 'dd/MM/yyyy')"; 
            if (AorP.ToUpper() == "P")
            {
                Query += " and h.voucherno is null ";
            }

            

            List<PayableDetails> objDetails = new List<PayableDetails>();
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
            OleDbConnection oraConnection = new OleDbConnection(connectionString);
            oraConnection.Open();
            OleDbDataAdapter oraAdapter = new OleDbDataAdapter(Query, oraConnection);
            oraConnection.Close();
            DataTable dt = new DataTable();
            oraAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PayableDetails details = new PayableDetails();
                    details.Department = dt.Rows[i][0].ToString();
                    details.PR_No = dt.Rows[i][1].ToString();
                    details.ServiceDetails = dt.Rows[i][2].ToString();
                    details.ExpanseHead = dt.Rows[i][3].ToString();
                    details.Panel = dt.Rows[i][4].ToString();
                    details.Price = dt.Rows[i][5].ToString();
                    details.Cost = dt.Rows[i][6].ToString();
                    details.Discount = dt.Rows[i][7].ToString();
                    details.Sales_Tax = dt.Rows[i][8].ToString();
                    details.Insurance_Amount = dt.Rows[i][9].ToString();
                    details.CostHead = dt.Rows[i][10].ToString();
                    details.ReferenceNo = dt.Rows[i][11].ToString();
                    details.RefundType = "R";
                    details.EnteredBy = dt.Rows[i][13].ToString();
                    details.EnteredOn = dt.Rows[i][14].ToString();
                    details.SubdepartmentId = dt.Rows[i][15].ToString();
                    if (string.IsNullOrEmpty(dt.Rows[i][16].ToString()))
                    {
                        details.ConsAff = null;
                    }
                    else
                    {
                        details.ConsAff = dt.Rows[i][16].ToString();
                    }

                    objDetails.Add(details);
                }

            }
            return objDetails;
        }

        private List<PayableDetails> GetGeneralRefund(string fromDate, string toDate, string AorP)
        {
            string Query = @"select 'General Service' as dept,
                                p.PRNO as prno,
                                m.revid as service_detail,
                                m.revcode as expense_head,
                                e.CompanyName as panel,
                                c.recamount as price,
                                c.totalamount as cost,
                                c.discount,
                                0 as sales_tax,
                                nvl(c.staff_discount,0) as insurance_amount,
                                g.description as cost_head,
                                f.refundno as referenceno,
                                u.name as refundtype,
                                vp.PersonName as enteredby,
                                f.refunddatetime as enteredOn,
                                '' as subdepartmentid
                                ,con.personid
                                   
                                from whims2.ac_txrevenue m
                                 left join whims2.hr_tconsultant con on con.revcode = m.revcode 
                                inner join whims2.ac_tcashreceived c on  m.revid=c.revid
                                inner join whims2.ac_tcashrefund f on f.paidno=c.paidno
                                inner join Hr_Trefundtype u on u.refundtypeid = f.refundtypeid
                                inner join whims2.pr_vpatientreg p on p.PRNO =c.prno
                                left outer join whims2.or_vemployee e on e.EMPLOYEEID=p.EMPLOYEEID
                                inner join whims2.ac_cashrechistory  h on trim(h.rno)=trim(f.refundno) and h.amount<0
                                and h.prno = p.PRNO
                                inner join Gl_Tcoa g on g.accountno = m.revcode
                                inner join whims2.hr_vpersonnel vp on vp.PERSONID = f.cashierid                          

                                where to_date(to_char(f.refunddatetime, 'dd/MM/yyyy'), 'dd/MM/yyyy') between
                                to_date('" + fromDate + @"', 'dd/MM/yyyy') and
                                to_date('" + toDate + @"', 'dd/MM/yyyy')";
            if (AorP.ToUpper() == "P")
            {
                Query += " and h.voucherno is null ";
            }

            Query += " order by p.prno";

            List<PayableDetails> objDetails = new List<PayableDetails>();
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
            OleDbConnection oraConnection = new OleDbConnection(connectionString);
            oraConnection.Open();
            OleDbDataAdapter oraAdapter = new OleDbDataAdapter(Query, oraConnection);
            oraConnection.Close();
            DataTable dt = new DataTable();
            oraAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PayableDetails details = new PayableDetails();
                    details.Department = dt.Rows[i][0].ToString();
                    details.PR_No = dt.Rows[i][1].ToString();
                    details.ServiceDetails = dt.Rows[i][2].ToString();
                    details.ExpanseHead = dt.Rows[i][3].ToString();
                    details.Panel = dt.Rows[i][4].ToString();
                    details.Price = dt.Rows[i][5].ToString();
                    details.Cost = dt.Rows[i][6].ToString();
                    details.Discount = dt.Rows[i][7].ToString();
                    details.Sales_Tax = dt.Rows[i][8].ToString();
                    details.Insurance_Amount = dt.Rows[i][9].ToString();
                    details.CostHead = dt.Rows[i][10].ToString();
                    details.ReferenceNo = dt.Rows[i][11].ToString();
                    details.RefundType = "R";
                    details.EnteredBy = dt.Rows[i][13].ToString();
                    details.EnteredOn = dt.Rows[i][14].ToString();
                    details.SubdepartmentId = dt.Rows[i][15].ToString();
                    if (string.IsNullOrEmpty(dt.Rows[i][16].ToString()))
                    {
                        details.ConsAff = null;
                    }
                    else
                    {
                        details.ConsAff = dt.Rows[i][16].ToString();
                    }

                    objDetails.Add(details);
                }

            }
            return objDetails;
        }

        private List<PayableDetails> GetPharmacyRefund(string fromDate, string toDate, string AorP)
        {
            List<PayableDetails> objDetails = new List<PayableDetails>();
            DataTable dt = GetOraRefund(fromDate, toDate, AorP);
            string PaidNoArray = GetStringForOra(dt);
            DataTable sqlTable = sqlDataTable(fromDate, toDate,PaidNoArray);
            int oraDataRowCount = dt.Rows.Count;

            for (int j = 0; j < dt.Rows.Count; j++)
            {
                for (int i = 0; i < sqlTable.Rows.Count; i++)
                {

                    if (sqlTable.Rows[i]["paidno"].ToString().Trim() == dt.Rows[j]["PAIDNO"].ToString().Trim())
                    {
                        PayableDetails detail = new PayableDetails();
                        detail.Department = dt.Rows[j][0].ToString();
                        detail.ExpanseHead = dt.Rows[j][1].ToString();
                        detail.Discount = dt.Rows[j][2].ToString();
                        detail.Insurance_Amount = dt.Rows[j][3].ToString();
                        detail.CostHead = dt.Rows[j][4].ToString();
                        detail.ReferenceNo = dt.Rows[j][5].ToString();
                        detail.EnteredBy = dt.Rows[j][6].ToString();
                        detail.EnteredOn = dt.Rows[j][7].ToString();
                        detail.RefundType = dt.Rows[j][8].ToString();

                        if (string.IsNullOrEmpty(dt.Rows[j][10].ToString()))
                        {
                            detail.ConsAff = null;
                        }
                        else
                        {
                            detail.ConsAff = "Y";
                        }


                        detail.Panel = "";
                        detail.Price = sqlTable.Rows[i][2].ToString();
                        detail.Sales_Tax = sqlTable.Rows[i][4].ToString();
                        detail.ServiceDetails = sqlTable.Rows[i][1].ToString();
                        detail.Cost = sqlTable.Rows[i][3].ToString();
                        detail.PR_No = sqlTable.Rows[i][0].ToString();
                        detail.SubdepartmentId = sqlTable.Rows[i][6].ToString();
                        objDetails.Add(detail);
                    }
                }
            }

            return objDetails;
        }

        private DataTable sqlDataTable(string fromDate, string toDate, string PaidNos)
        {
            /*string query = @"select m.PRNO,
                               md.medicineid as service_detail,
                               d.Qty * d.Price as price,
                               md.CostPrice as cost,
                               m.service_charge as sales_tax,
                               m.paidno,
                               m.SUBDEPTID
                          from PH_TPATIENT_REQUESTM m
                          join PH_TPatient_Return d on m.REQUESTID = d.REQUESTID
                          join ph_vMedicineName md on d.MEDICINEID = md.MEDICINEID
                          join HR_TSUBDEPARTMENT s on s.SUBDEPARTMENTID = m.SUBDEPTID
                         and m.paidno in (" + PaidNos + @") and m.paidno is not null
                         order by m.paidno";*/
            
            string query = @"select m.PRNO,
                               '-' as service_detail,
                               Round(sum(d.Qty * d.Price),0) as price,
                               Round(sum(d.Qty * d.Price),0) as cost,
                               '-' as sales_tax,
                               m.paidno,
                               m.SUBDEPTID
                          from PH_TPATIENT_REQUESTM m
                          join PH_TPatient_Return d on m.REQUESTID = d.REQUESTID
                          join ph_vMedicineName md on d.MEDICINEID = md.MEDICINEID
                          join HR_TSUBDEPARTMENT s on s.SUBDEPARTMENTID = m.SUBDEPTID
                         and m.paidno in (" + PaidNos + @") and m.paidno is not null group by m.prno,m.paidno,m.subdeptid,d.returnno
                         order by m.paidno";
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["sql"].ToString();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter dAdapter = new SqlDataAdapter(query, connectionString);
            connection.Close();
            DataTable dt = new DataTable();
            dAdapter.Fill(dt);
            return dt;
        }

        private DataTable GetOraRefund(string fromdate, string todate, string AorP)
        {
            string query = @"select 'Pharmacy' as dept,
                                   r.revcode as expense_head,
                                   t.discount,
                                   t.staff_discount as insurance_amount,
                                   c.description as cost_head,
                                   f.refundno as referenceno,
                                   vp.personname as enteredBy,
                                   t.enteredon as enteredOn,
                                   rt.name as refundtype,
                                   t.paidno as paidno
                                   ,con.personid
                                   from ac_tcashreceived t, ac_cashrechistory r
                                   left join whims2.hr_tconsultant con on con.revcode = r.revcode
                                   , gl_tcoa c, hr_vpersonnel vp
                                   ,ac_tcashrefund f, hr_trefundtype rt
                                   
  
                               where f.refundno = r.rno
                               and r.revcode = c.accountno
                               and f.cashierid = vp.PERSONID
                               and rt.refundtypeid=f.refundtypeid
                               and r.amount < 0
                               and t.paidno = f.paidno
                               and t.type = 'PH'
                               and to_date(to_char(r.edate, 'dd/MM/yyyy'), 'dd/MM/yyyy') between
                               to_date('" + fromdate + @"', 'dd/MM/yyyy') and
                               to_date('" + todate + @"', 'dd/MM/yyyy')";
            if (AorP.ToUpper() == "P")
            {
                query += " and r.voucherno is null";
            }
            query += " order by t.paidno";



            string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
            OleDbConnection oraConnection = new OleDbConnection(connectionString);
            oraConnection.Open();
            OleDbDataAdapter dAdapter = new OleDbDataAdapter(query, oraConnection);
            oraConnection.Close();
            DataTable dt = new DataTable();
            dAdapter.Fill(dt);
            return dt;
        }


        #endregion

        #region Private Methods For Receivables

        private List<Details> GetAllReceivables(string fromDate,string toDate,string AorP)
        {
 	        List<Details> _objDetails = new List<Details>();
            _objDetails.AddRange(GetPharmacy(fromDate,toDate,AorP));
            _objDetails.AddRange(GetPathology(fromDate,toDate,AorP));
            _objDetails.AddRange(GetCardiology(fromDate,toDate,AorP));
            _objDetails.AddRange(GetOPD(fromDate,toDate,AorP));
            _objDetails.AddRange(GetGeneral(fromDate,toDate,AorP));
            _objDetails.AddRange(GetRadiology(fromDate,toDate,AorP));
            _objDetails.AddRange(GetIPD(fromDate,toDate,AorP));
            return _objDetails;
            

        }

        private List<Details> GetOPD(string fromDate,string toDate,string AorP)
        {
            List<Details> objLabDetails = new List<Details>();

            string Query = @"select 'OPD' as dept,
                               p.PRNO as prno,
                               t.serviceid as service_detail,
                               s.revenuecode as expense_head,
                               e.CompanyName as panel,
                               t.rate as price,
                               t.rate as cost,
                               c.discount,
                               0 as sales_tax,
                               nvl(c.staff_discount,0) as insurance_amount,
                               g.description as cost_head,
                               c.paidno as referenceno,
                               vp.PersonName as enteredBy,
                               c.enteredon as enteredOn,
                               t.subdepartmentid
                               ,con.personid     
      
                               from whims2.pr_tpatientvisitm m
                               inner join whims2.pr_tpatientvisitd vd on trim(vd.visitno)=trim(m.visitno)
                               inner join whims2.hr_Tservice t on t.serviceid = vd.serviceid
                               inner join whims2.pr_vpatientreg p on p.PRNO = m.prno

                               inner join whims2.ac_tcashreceived c on  trim(m.visitno) = trim(c.referenceno) and vd.paidno=c.paidno
                               left outer join whims2.or_vemployee e on e.EMPLOYEEID=p.EMPLOYEEID
                               inner join whims2.ac_cashrechistory  h on trim(h.rno)=trim(c.paidno) and h.amount>=0
                               and h.prno = p.PRNO
                               inner join Hr_Tsubdepartment s on s.subdepartmentid=t.subdepartmentid
                               left join whims2.hr_tconsultant con on con.revcode = s.revenuecode
                               inner join Gl_Tcoa g on g.accountno = s.revenuecode
                               inner join whims2.hr_vpersonnel vp on vp.PERSONID = c.enteredby
                               where to_date(to_char(c.enteredon, 'dd/MM/yyyy'), 'dd/MM/yyyy') between
                               to_date('" + fromDate + @"', 'dd/MM/yyyy') and
                               to_date('" + toDate + @"', 'dd/MM/yyyy') ";
            if (AorP.ToUpper() == "P")
	        {
		        Query += " and h.voucherno is null ";
	        }
 
            Query +=" order by m.visitno";

            string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
            OleDbConnection oraConnection = new OleDbConnection(connectionString);
            oraConnection.Open();
            OleDbDataAdapter oraAdapter = new OleDbDataAdapter(Query, oraConnection);
            oraConnection.Close();
            DataTable dt = new DataTable();
            oraAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Details details = new Details();
                    details.Department = dt.Rows[i][0].ToString();
                    details.PR_No = dt.Rows[i][1].ToString();
                    details.ServiceDetails = dt.Rows[i][2].ToString();
                    details.ExpanseHead = dt.Rows[i][3].ToString();
                    details.Panel = dt.Rows[i][4].ToString();
                    details.Price = dt.Rows[i][5].ToString();
                    details.Cost = dt.Rows[i][6].ToString();
                    details.Discount = dt.Rows[i][7].ToString();
                    details.Sales_Tax = dt.Rows[i][8].ToString();
                    details.Insurance_Amount = dt.Rows[i][9].ToString();
                    details.CostHead = dt.Rows[i][10].ToString();
                    details.ReferenceNo = dt.Rows[i][11].ToString();
                    details.EnteredBy = dt.Rows[i][12].ToString();
                    details.EnteredOn = dt.Rows[i][13].ToString();
                    details.SubdepartmentId = dt.Rows[i][14].ToString();
                    if (string.IsNullOrEmpty(dt.Rows[i][15].ToString()))
                    {
                        details.ConsAff = null;
                    }
                    else
                    {
                        details.ConsAff = dt.Rows[i][15].ToString();
                    }

                    objLabDetails.Add(details);
                }

            }
            return objLabDetails;
        }

        private List<Details> GetIPD(string fromDate,string toDate,string AorP)
        {
 	        List<Details> objLabDetails = new List<Details>();

            string Query = @"select 'IN_Patient' as dept,
                                p.PRNO as prno,
                                t.packageid as service_detail,
                                t.revcode as expense_head,
                                e.CompanyName as panel,
                                d.paymentmade as price,
                                d.paymentmade as cost,
                                0 as discount,
                                0 as sales_tax,
                                nvl(d.staff_discount,0) as insurance_amount,
                                g.description as cost_head,
                                d.paidno as referenceno,
                                vp.PersonName as EnteredBy,
                                d.enteredon as EnteredOn,
                                t.departmentid                 
                                ,con.personid                                

                                from whims2.Bl_Tbillmaster m
                                inner join whims2.bl_tbilldetail d on m.billid = d.billid
                                inner join whims2.Hr_Tpackagemaster t on t.packageid = d.packageid
                                left join whims2.hr_tconsultant con on con.revcode = t.revcode
                                inner join whims2.pr_vpatientreg p on p.PRNO = m.prno
                                inner join whims2.Gl_Tcoa g on g.accountno=t.revcode
                                left outer join whims2.or_vemployee e on e.EMPLOYEEID=p.EMPLOYEEID
                                inner join whims2.hr_vpersonnel vp on vp.PERSONID = m.enteredby
                                where to_date(to_char(d.enteredon, 'dd/MM/yyyy'), 'dd/MM/yyyy') between
                                to_date('" + fromDate + @"', 'dd/MM/yyyy') and
                                to_date('" + toDate + @"', 'dd/MM/yyyy')
                                and d.paymentmade >0";
            if (AorP.ToUpper() == "P")
	        {
		        Query += " and d.voucherno is null ";
	        }
            Query += " order by m.billid";

             string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
             OleDbConnection oraConnection = new OleDbConnection(connectionString);
             oraConnection.Open();
             OleDbDataAdapter oraAdapter = new OleDbDataAdapter(Query, oraConnection);
             oraConnection.Close();
             DataTable dt = new DataTable();
             oraAdapter.Fill(dt);

             if (dt.Rows.Count > 0)
             {
                 for (int i = 0; i < dt.Rows.Count; i++)
                 {
                     Details details = new Details();
                     details.Department = dt.Rows[i][0].ToString();
                     details.PR_No = dt.Rows[i][1].ToString();
                     details.ServiceDetails = dt.Rows[i][2].ToString();
                     details.ExpanseHead = dt.Rows[i][3].ToString();
                     details.Panel = dt.Rows[i][4].ToString();
                     details.Price = dt.Rows[i][5].ToString();
                     details.Cost = dt.Rows[i][6].ToString();
                     details.Discount = dt.Rows[i][7].ToString();
                     details.Sales_Tax = dt.Rows[i][8].ToString();
                     details.Insurance_Amount = dt.Rows[i][9].ToString();
                     details.CostHead = dt.Rows[i][10].ToString();
                     details.ReferenceNo = dt.Rows[i][11].ToString();
                     details.EnteredBy = dt.Rows[i][12].ToString();
                     details.EnteredOn = dt.Rows[i][13].ToString();
                     details.SubdepartmentId = dt.Rows[i][14].ToString();
                     if (string.IsNullOrEmpty(dt.Rows[i][15].ToString()))
                     {
                         details.ConsAff = null;
                     }
                     else
                     {
                         details.ConsAff = dt.Rows[i][15].ToString();
                     }

                     objLabDetails.Add(details);
                 }

             }
             return objLabDetails;
        }

        private List<Details> GetRadiology(string fromDate,string toDate,string AorP)
        {
 	        List<Details> objLabDetails = new List<Details>();

            string Query = @"select 'Radiology' as dept,
                               p.PRNO as prno,
                               
                               t.invid as service_detail,
                               s.revenuecode as expense_head,
                               e.CompanyName as panel,
                               t.normalcharges as price,
                               t.normalcharges as cost,
                               c.discount,
                               0 as sales_tax,
                               nvl(c.staff_discount,0) as insurance_amount,
                               g.description as cost_head,
                               c.paidno as referenceno,
                               vp.PersonName as enteredBy,
                               c.enteredon as enteredOn,
                               t.subdepartmentid
                               ,con.personid     
      
                              from whims2.Rd_Ttransmaster m
                              inner join whims2.rd_ttransdetail d on m.transid = d.transid
                              inner join whims2.rd_tinvestigation t on t.invid = d.invid
                              inner join whims2.pr_vpatientreg p on p.PRNO = m.prno
                              inner join whims2.pr_tpatientvisitd vd on trim(vd.visitno)=trim(m.visitno) and p.prno=vd.prno
                              inner join whims2.ac_tcashreceived c on  trim(m.radiologyno) = trim(c.referenceno)
                              left outer join whims2.or_vemployee e on e.EMPLOYEEID=p.EMPLOYEEID
                              inner join whims2.ac_cashrechistory  h on trim(h.rno)=trim(c.paidno) and h.amount>=0
                              and h.prno = p.PRNO
                              inner join Hr_Tsubdepartment s on s.subdepartmentid=t.subdepartmentid
                              left join whims2.hr_tconsultant con on con.revcode = s.revenuecode
                              inner join Gl_Tcoa g on g.accountno = s.revenuecode
                              inner join whims2.hr_vpersonnel vp on vp.PERSONID = c.enteredby
                              where to_date(to_char(c.enteredon, 'dd/MM/yyyy'), 'dd/MM/yyyy') between
                              to_date('" + fromDate + @"', 'dd/MM/yyyy') and
                              to_date('" + toDate + @"', 'dd/MM/yyyy')
                              and m.patienttype = 'E' and m.paid='Y'";
            if (AorP.ToUpper() == "P")
	        {
		        Query += " and h.voucherno is null ";
	        }
            Query += " order by m.transid";

             string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
             OleDbConnection oraConnection = new OleDbConnection(connectionString);
             oraConnection.Open();
             OleDbDataAdapter oraAdapter = new OleDbDataAdapter(Query, oraConnection);
             oraConnection.Close();
             DataTable dt = new DataTable();
             oraAdapter.Fill(dt);

             if (dt.Rows.Count > 0)
             {
                 for (int i = 0; i < dt.Rows.Count; i++)
                 {
                     Details details = new Details();
                     details.Department = dt.Rows[i][0].ToString();
                     details.PR_No = dt.Rows[i][1].ToString();
                     details.ServiceDetails = dt.Rows[i][2].ToString();
                     details.ExpanseHead = dt.Rows[i][3].ToString();
                     details.Panel = dt.Rows[i][4].ToString();
                     details.Price = dt.Rows[i][5].ToString();
                     details.Cost = dt.Rows[i][6].ToString();
                     details.Discount = dt.Rows[i][7].ToString();
                     details.Sales_Tax = dt.Rows[i][8].ToString();
                     details.Insurance_Amount = dt.Rows[i][9].ToString();
                     details.CostHead = dt.Rows[i][10].ToString();
                     details.ReferenceNo = dt.Rows[i][11].ToString();
                     details.EnteredBy = dt.Rows[i][12].ToString();
                     details.EnteredOn = dt.Rows[i][13].ToString();
                     details.SubdepartmentId = dt.Rows[14].ToString();
                     if (string.IsNullOrEmpty(dt.Rows[i][15].ToString()))
                     {
                         details.ConsAff = null;
                     }
                     else
                     {
                         details.ConsAff = dt.Rows[i][15].ToString();
                     }

                     objLabDetails.Add(details);
                 }

             }
             return objLabDetails;
        }

        private List<Details> GetGeneral(string fromDate,string toDate,string AorP)
        {
 	        List<Details> objLabDetails = new List<Details>();

            string Query = @"select 'General Service' as dept,
                                   p.PRNO as prno,
                                   m.revid as service_detail,
                                   m.revcode as expense_head,
                                   e.CompanyName as panel,
                                   c.recamount as price,
                                   c.totalamount as cost,
                                   c.discount,
                                   0 as sales_tax,
                                   nvl(c.staff_discount,0) as insurance_amount,
                                   g.description as cost_head,
                                   c.paidno as referenceno,
                                   vp.PersonName as enteredBy,
                                   c.enteredon as enteredOn,
                                   '' as subdepartmentid,
                                    con.personid
                                         
                                   from whims2.ac_txrevenue m 
                                   left join whims2.hr_tconsultant con on con.revcode = m.revcode
                                   inner join whims2.ac_tcashreceived c on  m.revid=c.revid
                                   inner join whims2.pr_vpatientreg p on p.PRNO =c.prno
                                   left outer join whims2.or_vemployee e on e.EMPLOYEEID=p.EMPLOYEEID
                                   inner join whims2.ac_cashrechistory  h on trim(h.rno)=trim(c.paidno) and h.amount>=0
                                   and h.prno = p.PRNO
                                   inner join Gl_Tcoa g on g.accountno = m.revcode
                                   inner join whims2.hr_vpersonnel vp on vp.PERSONID = c.enteredby
                                   where to_date(to_char(c.enteredon, 'dd/MM/yyyy'), 'dd/MM/yyyy') between
                                   to_date('" + fromDate + @"', 'dd/MM/yyyy') and
                                   to_date('" + toDate + @"', 'dd/MM/yyyy') and c.type='G'";
            if (AorP.ToUpper() == "P")
	        {
                Query += " and h.voucherno is null ";
	        }
	                                  
            Query += " order by p.prno";

             string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
             OleDbConnection oraConnection = new OleDbConnection(connectionString);
             oraConnection.Open();
             OleDbDataAdapter oraAdapter = new OleDbDataAdapter(Query, oraConnection);
             oraConnection.Close();
             DataTable dt = new DataTable();
             oraAdapter.Fill(dt);

             if (dt.Rows.Count > 0)
             {
                 for (int i = 0; i < dt.Rows.Count; i++)
                 {
                     Details details = new Details();
                     details.Department = dt.Rows[i][0].ToString();
                     details.PR_No = dt.Rows[i][1].ToString();
                     details.ServiceDetails = dt.Rows[i][2].ToString();
                     details.ExpanseHead = dt.Rows[i][3].ToString();
                     details.Panel = dt.Rows[i][4].ToString();
                     details.Price = dt.Rows[i][5].ToString();
                     details.Cost = dt.Rows[i][6].ToString();
                     details.Discount = dt.Rows[i][7].ToString();
                     details.Sales_Tax = dt.Rows[i][8].ToString();
                     details.Insurance_Amount = dt.Rows[i][9].ToString();
                     details.CostHead = dt.Rows[i][10].ToString();
                     details.ReferenceNo = dt.Rows[i][11].ToString();
                     details.EnteredBy = dt.Rows[i][12].ToString();
                     details.EnteredOn = dt.Rows[i][13].ToString();
                     details.SubdepartmentId = dt.Rows[i][14].ToString();
                     if (string.IsNullOrEmpty(dt.Rows[i][15].ToString()))
                     {
                         details.ConsAff = null;
                     }
                     else
                     {
                         details.ConsAff = dt.Rows[i][15].ToString();
                     }

                     objLabDetails.Add(details);
                 }

             }
             return objLabDetails;
        }

        private List<Details> GetCardiology(string fromDate,string toDate,string AorP)
        {
 	        List<Details> objLabDetails = new List<Details>();

            string Query = @"select 'Cardiology' as dept,
                                    p.PRNO as prno,
                                    t.testid as service_detail,
                                    s.revenuecode as expense_head,
                                    e.CompanyName as panel,
                                    t.norcharges as price,
                                    t.norcharges as cost,
                                    c.discount,
                                    0 as sales_tax,
                                    nvl(c.staff_discount,0) as insurance_amount,
                                    g.description as cost_head,
                                    c.paidno as referenceno,
                                    vp.PersonName as enteredBy,
                                    c.enteredon as enteredOn,
                                    t.subdepartmentid
                                    ,con.personid
      
                                    from whims2.ca_trequestmaster m
                                    inner join whims2.ca_trequestdetail d on m.requestid = d.requestid
                                    inner join whims2.Ca_Ttestregistration t on t.testid = d.testid
                                    inner join whims2.pr_vpatientreg p on p.PRNO = m.prno
                                    inner join whims2.pr_tpatientvisitd vd on trim(vd.visitno)=trim(m.visitno) and p.prno=vd.prno
                                    inner join whims2.ac_tcashreceived c on  trim(m.requestno) = trim(c.referenceno)
                                    left outer join whims2.or_vemployee e on e.EMPLOYEEID=p.EMPLOYEEID
                                    inner join whims2.ac_cashrechistory  h on trim(h.rno)=trim(c.paidno) and h.amount>=0
                                    and h.prno = p.PRNO
                                    inner join Hr_Tsubdepartment s on s.subdepartmentid=t.subdepartmentid
                                    left join whims2.hr_tconsultant con on con.revcode = s.revenuecode
                                    inner join Gl_Tcoa g on g.accountno = s.revenuecode
                                    inner join whims2.hr_vpersonnel vp on vp.PERSONID = c.enteredby

                                    where to_date(to_char(c.enteredon, 'dd/MM/yyyy'), 'dd/MM/yyyy') between
                                    to_date('" + fromDate + @"', 'dd/MM/yyyy') and
                                    to_date('" + toDate + @"', 'dd/MM/yyyy')
                                    and m.ptype = 'O' and m.paid='Y'";
                            if (AorP.ToUpper() == "P")
	                        {
		                       Query += " and h.voucherno is null "; 
	                        }
                            Query +=  "order by m.requestid";
           
            
             string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
             OleDbConnection oraConnection = new OleDbConnection(connectionString);
             oraConnection.Open();
             OleDbDataAdapter oraAdapter = new OleDbDataAdapter(Query, oraConnection);
             oraConnection.Close();
             DataTable dt = new DataTable();
             oraAdapter.Fill(dt);

             if (dt.Rows.Count > 0)
             {
                 for (int i = 0; i < dt.Rows.Count; i++)
                 {
                     Details details = new Details();
                     details.Department = dt.Rows[i][0].ToString();
                     details.PR_No = dt.Rows[i][1].ToString();
                     details.ServiceDetails = dt.Rows[i][2].ToString();
                     details.ExpanseHead = dt.Rows[i][3].ToString();
                     details.Panel = dt.Rows[i][4].ToString();
                     details.Price = dt.Rows[i][5].ToString();
                     details.Cost = dt.Rows[i][6].ToString();
                     details.Discount = dt.Rows[i][7].ToString();
                     details.Sales_Tax = dt.Rows[i][8].ToString();
                     details.Insurance_Amount = dt.Rows[i][9].ToString();
                     details.CostHead = dt.Rows[i][10].ToString();
                     details.ReferenceNo = dt.Rows[i][11].ToString();
                     details.EnteredBy = dt.Rows[i][12].ToString();
                     details.EnteredOn = dt.Rows[i][13].ToString();
                     details.SubdepartmentId = dt.Rows[i][14].ToString();
                     if (string.IsNullOrEmpty(dt.Rows[i][15].ToString()))
                     {
                         details.ConsAff = null;
                     }
                     else
                     {
                         details.ConsAff = dt.Rows[i][15].ToString();
                     }

                     objLabDetails.Add(details);
                 }

             }
             return objLabDetails;
        }

        private List<Details> GetPathology(string fromDate,string toDate, string AorP)
        {
 	        List<Details> objLabDetails = new List<Details>();

            string Query = @"select 'Pathology' as dept,
                               p.PRNO as prno,
                               
                               t.testid as service_detail,
                               '3-01-01-04-002-001-1' as expense_head,
                               e.CompanyName as panel,
                               d.charges as price,
                               d.charges as cost,
                               c.discount as discount,
                               0 as sales_tax,
                               nvl(c.staff_discount, 0) as insurance_amount,
                               'outpatient-laboratory' as cost_head,
                               c.paidno as referenceno,
                               vp.PersonName as enteredBy,
                               c.enteredon as enteredOn,
                               t.sectionid

                         from whims.Ls_Tmtransaction m
                         inner join whims.ls_tdtransaction d on m.mserialno = d.mserialno
                         inner join whims.ls_ttest t on t.testid = d.testid
                         inner join whims2.pr_vpatientreg p on trim(p.PRNO) = trim(m.prno)
                         inner join whims2.ac_tcashreceived c on c.paidno = m.paidno
                                                             and trim(c.referenceno) =
                                                                 trim(m.labid)
                         left outer join whims2.or_vemployee e on e.EMPLOYEEID = p.EMPLOYEEID
                         inner join whims2.ac_cashrechistory h on h.rno = c.paidno
                                                              and h.amount >= 0
                                                              and h.prno = p.PRNO
                         inner join whims2.hr_vpersonnel vp on vp.PERSONID = c.enteredby

                         where to_date(to_char(c.enteredon, 'dd/MM/yyyy'), 'dd/MM/yyyy') between
                               to_date('" + fromDate + @"', 'dd/MM/yyyy') and
                               to_date('" + toDate + @"', 'dd/MM/yyyy')
                           and m.iop = 'O'";
            if (AorP.ToUpper() == "P")
            {
                Query += " and h.voucherno is null";
            }

             string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
             OleDbConnection oraConnection = new OleDbConnection(connectionString);
             oraConnection.Open();
             OleDbDataAdapter oraAdapter = new OleDbDataAdapter(Query, oraConnection);
             oraConnection.Close();
             DataTable dt = new DataTable();
             oraAdapter.Fill(dt);

             if (dt.Rows.Count > 0)
             {
                 for (int i = 0; i < dt.Rows.Count; i++)
                 {
                     Details details = new Details();
                     details.Department = dt.Rows[i][0].ToString();
                     details.PR_No = dt.Rows[i][1].ToString();
                     details.ServiceDetails = dt.Rows[i][2].ToString();
                     details.ExpanseHead = dt.Rows[i][3].ToString();
                     details.Panel = dt.Rows[i][4].ToString();
                     details.Price = dt.Rows[i][5].ToString();
                     details.Cost = dt.Rows[i][6].ToString();
                     details.Discount = dt.Rows[i][7].ToString();
                     details.Sales_Tax = dt.Rows[i][8].ToString();
                     details.Insurance_Amount = dt.Rows[i][9].ToString();
                     details.CostHead = dt.Rows[i][10].ToString();
                     details.ReferenceNo = dt.Rows[i][11].ToString();
                     details.EnteredBy = dt.Rows[i][12].ToString();
                     details.EnteredOn = dt.Rows[i][13].ToString();
                     details.SubdepartmentId = dt.Rows[i][14].ToString();

                     objLabDetails.Add(details);
                 }

             }
             return objLabDetails;
        }

        private List<Details> GetPharmacy(string fromDate, string toDate, string AorP)
        {
            DataTable oraDataTable = GetOracleData(fromDate, toDate, AorP);
            DataTable sqlDataTable = GetSqlServerData(fromDate, toDate);
            List<Details> objLabDetails = new List<Details>();
            //int oraDataRowCount = oraDataTable.Rows.Count;

            for (int j = 0; j < oraDataTable.Rows.Count; j++)
            {

                for (int i = 0; i < sqlDataTable.Rows.Count; i++)
                {
                    if (sqlDataTable.Rows[i]["PAIDNO"].ToString().Trim() == oraDataTable.Rows[j]["paidno"].ToString().Trim())
                    {
                        Details details = new Details();
                        details.Department = sqlDataTable.Rows[i]["dept"].ToString();
                        details.PR_No = sqlDataTable.Rows[i]["PRNO"].ToString();
                        details.ServiceDetails = sqlDataTable.Rows[i]["service_detail"].ToString();
                        details.ExpanseHead = oraDataTable.Rows[j]["expense_head"].ToString();
                        details.Panel = "";//sqlDataTable.Rows[i]["????????"].ToString();
                        details.Price = sqlDataTable.Rows[i]["price"].ToString();
                        details.Cost = sqlDataTable.Rows[i]["cost"].ToString();
                        details.Discount = sqlDataTable.Rows[i]["discount"].ToString();
                        details.Sales_Tax = sqlDataTable.Rows[i]["sales_tax"].ToString();
                        details.Insurance_Amount = oraDataTable.Rows[j]["insurance_amount"].ToString();
                        details.CostHead = oraDataTable.Rows[j]["cost_head"].ToString();
                        details.ReferenceNo = oraDataTable.Rows[j]["paidno"].ToString();
                        details.EnteredBy = oraDataTable.Rows[j]["enteredBy"].ToString();
                        details.EnteredOn = oraDataTable.Rows[j]["enteredOn"].ToString();
                        details.SubdepartmentId = sqlDataTable.Rows[i]["SUBDEPTID"].ToString();
                        if (string.IsNullOrEmpty(oraDataTable.Rows[j]["personid"].ToString()))
                        {
                            details.ConsAff = null;
                        }
                        else
                        {
                            details.ConsAff = oraDataTable.Rows[j]["personid"].ToString();
                        }

                        objLabDetails.Add(details);
                    }
                }


            }

            return objLabDetails;


        }

        private DataTable GetSqlServerData(string fromDate, string toDate)
        {
            string query = @"select 'Pharmacy' as dept,m.PRNO,md.medicineid as service_detail,
                                    m.TOTALAMOUNT as price, md.CostPrice as cost, 0 as discount, m.service_charge as sales_tax
                                    ,m.PAIDNO,m.SUBDEPTID
                                    from PH_TPATIENT_REQUESTM m
                                    join PH_TPATIENT_REQUESTD d on m.REQUESTID = d.REQUESTID
                                    join ph_vMedicineName md on d.MEDICINEID = md.MEDICINEID 
                                    join HR_TSUBDEPARTMENT s on s.SUBDEPARTMENTID = m.SUBDEPTID
                                    where convert(date,m.enteredon,103) between convert(date,'" + fromDate + @"',103) and convert(date,'" + toDate + @"',103)
                                    order by m.paidno";
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["sql"].ToString();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter dAdapter = new SqlDataAdapter(query, connectionString);
            connection.Close();
            DataTable dt = new DataTable();
            dAdapter.Fill(dt);
            return dt;

        }

        private DataTable GetOracleData(string fromDate, string toDate, string AorP)
        {
            string query = @"select r.revcode as expense_head,
                                   t.discount,
                                   t.staff_discount as insurance_amount,
                                   c.description as cost_head,
                                   t.paidno,
                                   vp.personname as enteredBy,
                                   t.enteredon as enteredOn,
                                   con.personid
                                    
                                   from ac_tcashreceived t, ac_cashrechistory r
                                   left join whims2.hr_tconsultant con on con.revcode = r.revcode,
                                   gl_tcoa c, hr_vpersonnel vp
  
                               where t.paidno = r.rno
                               and r.revcode = c.accountno
                               and r.amount > 0
                               and t.enteredby = vp.PERSONID
                               and t.type = 'PH'
                               and to_date(to_char(t.enteredon, 'dd/MM/yyyy'), 'dd/MM/yyyy') between
                               to_date('" + fromDate + @"', 'dd/MM/yyyy') and
                               to_date('" + toDate + @"', 'dd/MM/yyyy')";
            if (AorP.ToUpper() == "P")
            {
                query += " and r.voucherno is null";
            }
            query += " order by t.paidno";

            string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
            OleDbConnection oraConnection = new OleDbConnection(connectionString);
            oraConnection.Open();
            OleDbDataAdapter dAdapter = new OleDbDataAdapter(query, oraConnection);
            oraConnection.Close();
            DataTable dt = new DataTable();
            dAdapter.Fill(dt);
            return dt;



        }

        #endregion

        #region Private Method For Consumption

        private List<ConsumptionDetails> VisitsConsumption(string fromDate, string toDate)
        {

            List<ConsumptionDetails> objDetails = new List<ConsumptionDetails>();
            string Query = @"select t.admno,t.tserviceid as service_detail,t.charges, '3-01-01-14-001-001-1' as expense_head,'Inpatient physiotherapy services' as                                  costhead
                              from wd_vservices t 
                              where  t.departmentid = '050'
                              and to_date(to_char(t.service_date, 'DD/MM/YYYY'),
                              'DD/MM/YYYY') between
                              to_date('" + fromDate + "', 'DD/MM/YYYY') and to_date('" + toDate + "', 'DD/MM/YYYY')";

            string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
            OleDbConnection oraConnection = new OleDbConnection(connectionString);
            oraConnection.Open();
            OleDbDataAdapter oraAdapter = new OleDbDataAdapter(Query, oraConnection);
            oraConnection.Close();
            DataTable dt = new DataTable();
            oraAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ConsumptionDetails details = new ConsumptionDetails();
                    details.AdmissionNo = dt.Rows[i][0].ToString();
                    details.ServiceDetails = dt.Rows[i][1].ToString();
                    details.Charges = dt.Rows[i][2].ToString();
                    details.ExpanseHead = dt.Rows[i][3].ToString();
                    details.CostHead = dt.Rows[i][4].ToString();
                    objDetails.Add(details);
                }
            }

            objDetails.AddRange(GetCV(fromDate, toDate));

            return objDetails;

        }

        private List<ConsumptionDetails> GetCV(string fromDate, string toDate)
        {
            List<ConsumptionDetails> objDetails = new List<ConsumptionDetails>();
            string Query = @"select t.admno,t.tserviceid as service_detail,t.charges,t.revenuecode as expense_head,c.description as costhead,con.personid
                              from wd_vservices t
                               left join hr_tconsultant con on con.revcode = t.revenuecode
                               ,gl_tcoa c
                              where t.revenuecode=c.accountno and t.departmentid = '001'
                              and to_date(to_char(t.service_date, 'DD/MM/YYYY'),
                              'DD/MM/YYYY') between
                              to_date('" + fromDate + "', 'DD/MM/YYYY') and to_date('" + toDate + "', 'DD/MM/YYYY')";

            string connectionString = System.Configuration.ConfigurationManager.AppSettings["orc"].ToString();
            OleDbConnection oraConnection = new OleDbConnection(connectionString);
            oraConnection.Open();
            OleDbDataAdapter oraAdapter = new OleDbDataAdapter(Query, oraConnection);
            oraConnection.Close();
            DataTable dt = new DataTable();
            oraAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ConsumptionDetails details = new ConsumptionDetails();
                    details.AdmissionNo = dt.Rows[i][0].ToString();
                    details.ServiceDetails = dt.Rows[i][1].ToString();
                    details.Charges = dt.Rows[i][2].ToString();
                    details.ExpanseHead = dt.Rows[i][3].ToString();
                    details.CostHead = dt.Rows[i][4].ToString();
                    if (string.IsNullOrEmpty(dt.Rows[i][5].ToString()))
                    {
                        details.ConsAff = null;
                    }
                    else
                    {
                        details.ConsAff = dt.Rows[i][5].ToString();
                    }
                    objDetails.Add(details);
                }
            }
            return objDetails;
        }

        private List<ConsumptionDetails> WardandFloorStockConsumption(string fromDate, string toDate)
        {
            List<ConsumptionDetails> _objDetails = new List<ConsumptionDetails>();

            string query = @"Select rm.ADMISSIONNO as admno,
                               
                               m.medicineid as service_detail,
                               rd.TOTALAMOUNT as charges,
                               rm.service_charge as service_charges,
                               s.REVENUECODE as expense_head,
                               c.description as costhead,
                               con.personid
                          from ph_Tpatient_requestm rm
                          join ph_Tpatient_requestd rd on rm.requestid = rd.requestid
                          join ph_tmed_master m on m.medicineid = rd.medicineid
                          join PH_TUNIT u on m.UNITID = u.UNITID
                          join PH_TNATURE n on n.NATUREID = m.NATUREID
                          join HR_TSUBDEPARTMENT s on s.SUBDEPARTMENTID = rm.SUBDEPTID
                           left join hr_tconsultant con on con.revcode = s.revenuecode
                          left outer join GL_TCOA c on c.accountno = s.REVENUECODE
                         where 1 = 1
                           and rd.qty_issued is not null
                           and rm.status = 'I'
                           and rm.subdeptid in ('0164', '0316')
                           and convert(date, rm.enteredon, 103) between
                               convert(date, '" + fromDate + @"', 103) and
                               convert(date, '" + toDate + "', 103)";
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["sql"].ToString();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter dAdapter = new SqlDataAdapter(query, connectionString);
            connection.Close();
            DataTable dt = new DataTable();
            dAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ConsumptionDetails details = new ConsumptionDetails();
                    details.AdmissionNo = dt.Rows[i][0].ToString();
                    details.ServiceDetails = dt.Rows[i][1].ToString();
                    details.Charges = dt.Rows[i][2].ToString();
                    details.ExpanseHead = dt.Rows[i][3].ToString();
                    details.CostHead = dt.Rows[i][4].ToString();
                    if (string.IsNullOrEmpty(dt.Rows[i][5].ToString()))
                    {
                        details.ConsAff = null;
                    }
                    else
                    {
                        details.ConsAff = dt.Rows[i][5].ToString();
                    }
                    _objDetails.Add(details);
                }
            }
            _objDetails.AddRange(GetFloorStockConsumption(fromDate, toDate));

            return _objDetails;
        }

        private List<ConsumptionDetails> GetFloorStockConsumption(string fromDate, string toDate)
        {
            List<ConsumptionDetails> _objDetails = new List<ConsumptionDetails>();

            string query = @"Select rm.ADMISSIONNO as admno,
       
                                    m.medicineid      as service_detail,
                                    rd.TOTALAMOUNT    as charges,
                                    rm.service_charge as service_charges,
                                    s.REVENUECODE     as expense_head,
                                    c.description     as costhead,
                                    con.personid
                                from ph_Tpatient_requestm rm
                                join ph_Tpatient_requestd rd on rm.requestid = rd.requestid
                                join ph_tmed_master m on m.medicineid = rd.medicineid
                                join PH_TUNIT u on m.UNITID = u.UNITID
                                join PH_TNATURE n on n.NATUREID = m.NATUREID
                                left outer join PH_TPatient_Return r on r.Rx_No = rd.RXNO
                                                                    and r.StatusId = 'A'
                                join PH_TDEPTCONSUMPTIONM dm on dm.PrescriptionNo = rm.PRESCRIPTION_NO
                                inner join HR_TSUBDEPARTMENT s on s.SUBDEPARTMENTID = rm.SUBDEPTID
                                left join hr_tconsultant con on con.revcode = s.revenuecode
                                left outer join GL_TCOA c on c.accountno = s.REVENUECODE
                                where 1 = 1
                                and rd.qty_issued is not null
      
                                and rm.status = 'I'
                                and convert(date, rm.enteredon, 103) between
                                convert(date, '" + fromDate + @"', 103) and
                                convert(date, '" + toDate + "', 103)";
                              
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["sql"].ToString();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter dAdapter = new SqlDataAdapter(query, connectionString);
            connection.Close();
            DataTable dt = new DataTable();
            dAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ConsumptionDetails details = new ConsumptionDetails();
                    details.AdmissionNo = dt.Rows[i][0].ToString();
                    details.ServiceDetails = dt.Rows[i][1].ToString();
                    details.Charges = dt.Rows[i][2].ToString();
                    details.ExpanseHead = dt.Rows[i][3].ToString();
                    details.CostHead = dt.Rows[i][4].ToString();
                    if (string.IsNullOrEmpty(dt.Rows[i][5].ToString()))
                    {
                        details.ConsAff = null;
                    }
                    else
                    {
                        details.ConsAff = dt.Rows[i][5].ToString();
                    }
                    _objDetails.Add(details);
                }
            }
            return _objDetails;
        }

        private List<ConsumptionDetails> CathConsumption(string fromDate, string toDate)
        {

            List<ConsumptionDetails> _objDetails = new List<ConsumptionDetails>();
            string Query = @"Select rm.ADMISSIONNO as admno,
                                    
                                    m.medicineid as service_detail,
                                    rd.TOTALAMOUNT as charges,
                                    rm.service_charge
       
                                    as service_charges,
                                    s.REVENUECODE as expense_head,
                                    c.description as costhead,
                                    con.personid
                                from ph_Tpatient_requestm rm
                                join ph_Tpatient_requestd rd on rm.requestid = rd.requestid
                                join ph_tmed_master m on m.medicineid = rd.medicineid
                                join PH_TUNIT u on m.UNITID = u.UNITID
                                join PH_TNATURE n on n.NATUREID = m.NATUREID
                                join HR_TSUBDEPARTMENT s on s.SUBDEPARTMENTID = rm.SUBDEPTID
                                left join hr_tconsultant con on con.revcode = s.revenuecode
                                left outer join GL_TCOA c on c.accountno = s.REVENUECODE
                                left outer join PH_TPatient_Return r on r.Rx_No = rd.RXNO
                                                                    and r.StatusId = 'A'
                                where 1 = 1
                                and rd.qty_issued is not null
                                and rm.status = 'I'
                                and rm.subdeptid in ('0217')
                                and convert(date, rm.enteredon, 103) between
                                    convert(date, '" + fromDate + @"', 103) and
                                    convert(date, '" + toDate + "', 103)";

            string connectionString = System.Configuration.ConfigurationManager.AppSettings["sql"].ToString();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter dAdapter = new SqlDataAdapter(Query, connectionString);
            connection.Close();
            DataTable dt = new DataTable();
            dAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ConsumptionDetails details = new ConsumptionDetails();
                    details.AdmissionNo = dt.Rows[i][0].ToString();
                    details.ServiceDetails = dt.Rows[i][1].ToString();
                    details.Charges = dt.Rows[i][2].ToString();
                    details.ExpanseHead = dt.Rows[i][3].ToString();
                    details.CostHead = dt.Rows[i][4].ToString();
                    if (string.IsNullOrEmpty(dt.Rows[i][5].ToString()))
                    {
                        details.ConsAff = null;
                    }
                    else
                    {
                        details.ConsAff = dt.Rows[i][5].ToString();
                    }
                    _objDetails.Add(details);
                }
            }
            return _objDetails;


        }

        private List<ConsumptionDetails> GetOTPharmacyConsumption(string fromDate, string toDate)
        {
            List<ConsumptionDetails> _objDetails = new List<ConsumptionDetails>();
            string Query = @"Select rm.ADMISSIONNO as admno,
                                   
                                    m.medicineid as service_detail,
                                   rd.TOTALAMOUNT as charges,
                                   rm.service_charge
       
                                   as service_charges,
                                   s.REVENUECODE as expense_head,
                                   c.description as costhead,
                                    con.personid
                              from ph_Tpatient_requestm rm
                              join ph_Tpatient_requestd rd on rm.requestid = rd.requestid
                              join ph_tmed_master m on m.medicineid = rd.medicineid
                              join PH_TUNIT u on m.UNITID = u.UNITID
                              join PH_TNATURE n on n.NATUREID = m.NATUREID
                              join HR_TSUBDEPARTMENT s on s.SUBDEPARTMENTID = rm.SUBDEPTID
                               left join hr_tconsultant con on con.revcode = s.revenuecode
                              left outer join GL_TCOA c on c.accountno = s.REVENUECODE
                              left outer join PH_TPatient_Return r on r.Rx_No = rd.RXNO
                                                                  and r.StatusId = 'A'
                             where 1 = 1
                               and rd.qty_issued is not null
                               and rm.status = 'I'
                               and rm.subdeptid in ('0218', '0219', '0288', '0289', '0330', '0317')
                               and convert(date, rm.enteredon, 103) between
                                   convert(date, '" + fromDate + @"', 103) and
                                   convert(date, '" + toDate + "', 103)";

            string connectionString = System.Configuration.ConfigurationManager.AppSettings["sql"].ToString();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter dAdapter = new SqlDataAdapter(Query, connectionString);
            connection.Close();
            DataTable dt = new DataTable();
            dAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ConsumptionDetails details = new ConsumptionDetails();
                    details.AdmissionNo = dt.Rows[i][0].ToString();
                    details.ServiceDetails = dt.Rows[i][1].ToString();
                    details.Charges = dt.Rows[i][2].ToString();
                    details.ExpanseHead = dt.Rows[i][3].ToString();
                    details.CostHead = dt.Rows[i][4].ToString();
                    if (string.IsNullOrEmpty(dt.Rows[i][5].ToString()))
                    {
                        details.ConsAff = null;
                    }
                    else
                    {
                        details.ConsAff = dt.Rows[i][5].ToString();
                    }
                    _objDetails.Add(details);
                }
            }
            return _objDetails;
        }

        private List<ConsumptionDetails> GetFloorStockReturn(string fromDate, string toDate)
        {
            List<ConsumptionDetails> _objDetails = new List<ConsumptionDetails>();

            string query = @"Select rm.ADMISSIONNO as admno,
                                   
                                   m.medicineid as service_detail,
                                   rd.TOTALAMOUNT as charges,
                                   rm.service_charge
       
                                   as service_charges,
                                   s.REVENUECODE as expense_head,
                                   c.description as costhead,
                                    con.personid
                              from ph_Tpatient_requestm rm
                              join ph_Tpatient_requestd rd on rm.requestid = rd.requestid
                              join ph_tmed_master m on m.medicineid = rd.medicineid
                              join PH_TUNIT u on m.UNITID = u.UNITID
                              join PH_TNATURE n on n.NATUREID = m.NATUREID
                              join PH_TPatient_Return r on r.Rx_No = rd.RXNO
                                                       and r.StatusId = 'A'
                              join PH_TDEPTCONSUMPTIONM dm on dm.PrescriptionNo = rm.PRESCRIPTION_NO
                              join HR_TSUBDEPARTMENT s on s.SUBDEPARTMENTID = rm.SUBDEPTID
                              left join hr_tconsultant con on con.revcode = s.revenuecode
                              left outer join GL_TCOA c on c.accountno = s.REVENUECODE
                             where 1 = 1
                               and rd.qty_issued is not null
                               and rm.status = 'I'
                               and convert(date, r.enteredon, 103) between
                                   convert(date, '" + fromDate + @"', 103) and
                                   convert(date, '" + toDate + "', 103)";
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["sql"].ToString();
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlDataAdapter dAdapter = new SqlDataAdapter(query, connectionString);
            connection.Close();
            DataTable dt = new DataTable();
            dAdapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ConsumptionDetails details = new ConsumptionDetails();
                    details.AdmissionNo = dt.Rows[i][0].ToString();
                    details.ServiceDetails = dt.Rows[i][1].ToString();
                    details.Charges = dt.Rows[i][2].ToString();
                    details.ExpanseHead = dt.Rows[i][3].ToString();
                    details.CostHead = dt.Rows[i][4].ToString();
                    if (string.IsNullOrEmpty(dt.Rows[i][5].ToString()))
                    {
                        details.ConsAff = null;
                    }
                    else
                    {
                        details.ConsAff = dt.Rows[i][5].ToString();
                    }
                    _objDetails.Add(details);
                }
            }
            return _objDetails;
        }

        

        #endregion

        #endregion

        #region Test

        private List<PayableDetails> getPhRefund(string fromDate, string toDate, string AorP) 
        {
            List<PayableDetails> objDetails = new List<PayableDetails>();
            DataTable dt = GetOraRefund(fromDate, toDate, AorP);

            string PaidNoArray = GetStringForOra(dt);
            DataTable sqlTable = sqlDataTable(fromDate, toDate,PaidNoArray);
            DataTable PayableResultTable = new DataTable();
            var resultTable = (from t1 in dt.AsEnumerable()
                               join t2 in sqlTable.AsEnumerable()
                               on t1.Field<string>("PAIDNO") equals t2.Field<string>("paidno")
                               select new
                               {
                                   Department = t1["DEPT"].ToString(),
                                   ExpanseHead = t1["EXPENSE_HEAD"].ToString(),
                                   Discount = t1["DISCOUNT"].ToString(),
                                   InsuranceAmount = t1["INSURANCE_AMOUNT"].ToString(),
                                   CostHead = t1["COST_HEAD"].ToString(),
                                   RefundType = t1["REFUNDTYPE"].ToString(),
                                   ReferenceNo = t1["REFERENCENO"].ToString(),
                                   EnteredBy = t1["ENTEREDBY"].ToString(),
                                   EnteredOn = t1["ENTEREDON"].ToString(),
                                   Price = t2["price"].ToString(),
                                   SalesTax = t2["sales_tax"].ToString(),
                                   ServiceDetails = t2["service_detail"].ToString(),
                                   Cost = t2["cost"].ToString(),
                                   PR_NO = t2["PRNO"].ToString(),
                                   SID = t2["SUBDEPTID"].ToString(),
                                   PaidNo = t2["paidno"].ToString()
                               });
           
           
            foreach (var dRow in resultTable)
            {
                PayableDetails detail = new PayableDetails();
                detail.Department = dRow.Department;
                detail.ExpanseHead = dRow.ExpanseHead;
                detail.Discount = dRow.Discount;
                detail.Insurance_Amount = dRow.InsuranceAmount;
                detail.CostHead = dRow.CostHead;
                detail.ReferenceNo = dRow.ReferenceNo;
                detail.EnteredBy = dRow.EnteredBy;
                detail.EnteredOn = dRow.EnteredOn;
                detail.RefundType = dRow.RefundType;
                detail.ConsAff = null;
                detail.Panel = "";
                detail.Price = dRow.Price;
                detail.Sales_Tax = dRow.SalesTax;
                detail.ServiceDetails = dRow.ServiceDetails;
                detail.Cost = dRow.Cost;
                detail.PR_No = dRow.PR_NO;
                detail.SubdepartmentId = dRow.SID;
                objDetails.Add(detail);
            }

            return objDetails;
        }

        private string GetStringForOra(DataTable dt)
        {
            string paidNo = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i < dt.Rows.Count - 1)
                {
                    paidNo += "'" + dt.Rows[i]["PAIDNO"].ToString() + "',";
                }
                else
                {
                    paidNo += "'" + dt.Rows[i]["PAIDNO"].ToString() + "'";
                }
            }
            if (paidNo == "")
            {
                paidNo="''";
            }
            return paidNo;
        }

        #endregion

    }
}
