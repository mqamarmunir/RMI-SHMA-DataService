using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Services;
using System.ServiceModel.Web;

namespace RMI_DataService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IRMIDataService
    {
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped,
                                Method = "GET",
                                RequestFormat = WebMessageFormat.Json,
                                ResponseFormat = WebMessageFormat.Json,
                                UriTemplate = "/GetReceivableData/{flag}/{AorP}/{fromDate}/{toDate}")]
                                List<Details> GetReceivableData(string flag,string AorP,string fromDate,string toDate);
        
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped,
                                Method = "GET",
                                RequestFormat = WebMessageFormat.Json,
                                ResponseFormat = WebMessageFormat.Json,
                                UriTemplate = "/GetPayableDataRefund/{flag}/{AorP}/{fromDate}/{toDate}")]
        List<PayableDetails> GetPayableDataRefund(string flag, string AorP, string fromDate, string toDate);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped,
                                Method = "GET",
                                RequestFormat = WebMessageFormat.Json,
                                ResponseFormat = WebMessageFormat.Json,
                                UriTemplate = "/GetConsumption/{flag}/{fromDate}/{toDate}")]
        List<ConsumptionDetails> GetConsumption(string flag, string fromDate, string toDate);


        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped,
                                Method = "GET",
                                RequestFormat = WebMessageFormat.Json,
                                ResponseFormat = WebMessageFormat.Json,
                                UriTemplate = "/GetOTProcedureConsumption/{fromDate}/{toDate}")]
        List<OTConsumptionDetails> GetOTProcedureConsumption(string fromDate, string toDate);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped,
                                Method = "GET",
                                RequestFormat = WebMessageFormat.Json,
                                ResponseFormat = WebMessageFormat.Json,
                                UriTemplate = "/GetWardAndFloorStockReturn/{fromDate}/{toDate}")]
        List<ConsumptionDetails> GetWardAndFloorStockReturn(string fromDate, string toDate);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped,
            Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            UriTemplate = "/UpdateAC/{referenceNo}/{importNo}")]
        int UpdateAC(string referenceNo, string importNo);

        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Wrapped,
            Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            UriTemplate = "/UpdateBL/{referenceNo}/{importNo}")]
        int UpdateBL(string referenceNo, string importNo);    
    }

    [DataContract]
    public class Details
    {
        [DataMember]
        public string Department { get; set; }
        [DataMember]
        public string PR_No { get; set; }
        [DataMember]
        public string ServiceDetails { get; set; }
        [DataMember]
        public string ExpanseHead { get; set; }
        [DataMember]
        public string Panel { get; set; }
        [DataMember]
        public string Price { get; set; }
        [DataMember]
        public string Cost { get; set; }
        [DataMember]
        public string Discount { get; set; }
        [DataMember]
        public string Sales_Tax { get; set; }
        [DataMember]
        public string Insurance_Amount { get; set; }
        [DataMember]
        public string CostHead { get; set; }
        [DataMember]
        public string ReferenceNo { get; set; }
        [DataMember]
        public string EnteredBy { get; set; }
        [DataMember]
        public string EnteredOn { get; set; }
        [DataMember]
        public string SubdepartmentId { get; set; }
        [DataMember]
        public string ConsAff { get; set; }

        [DataContract]
        public class RootObject
        {
            public Details LabDetails { get; set; }
        }

    }

    [DataContract]
    public class PayableDetails
    {
        [DataMember]
        public string Department { get; set; }
        [DataMember]
        public string PR_No { get; set; }
        [DataMember]
        public string ServiceDetails { get; set; }
        [DataMember]
        public string ExpanseHead { get; set; }
        [DataMember]
        public string Panel { get; set; }
        [DataMember]
        public string Price { get; set; }
        [DataMember]
        public string Cost { get; set; }
        [DataMember]
        public string Discount { get; set; }
        [DataMember]
        public string Sales_Tax { get; set; }
        [DataMember]
        public string Insurance_Amount { get; set; }
        [DataMember]
        public string CostHead { get; set; }
        [DataMember]
        public string ReferenceNo { get; set; }
        [DataMember]
        public string RefundType { get; set; }
        [DataMember]
        public string EnteredBy { get; set; }
        [DataMember]
        public string EnteredOn { get; set; }
        [DataMember]
        public string SubdepartmentId { get; set; }
        [DataMember]
        public string ConsAff { get; set; }
       
        

        [DataContract]
        public class RootObject
        {
            public PayableDetails cashDetails { get; set; }
        }

        
    }

    [DataContract]
    public class OTConsumptionDetails
    {
        [DataMember]
        public string AdmissionNo { get; set; }
        [DataMember]
        public string PersonShare { get; set; }
        [DataMember]
        public string HospitalShare { get; set; }
        [DataMember]
        public string ExpenseHead { get; set; }
        [DataMember]
        public string CostHead { get; set; }
        [DataMember]
        public string ConsAff { get; set; }
        [DataContract]
        public class RootObject
        {
            public List<OTConsumptionDetails> consumptionDetails { get; set; }
        }
    }



    [DataContract]
    public class ConsumptionDetails
    { 
        [DataMember]
        public string AdmissionNo { get; set; }
        [DataMember]
        public string ServiceDetails { get; set; }
        [DataMember]
        public string Charges { get; set; }
        [DataMember]
        public string ExpanseHead { get; set; }
        [DataMember]
        public string CostHead { get; set; }
        [DataMember]
        public string ConsAff { get; set; }

        [DataContract]
        public class RootObject
        {
            public List<ConsumptionDetails> details { get; set; }
        }
        

    }

}
