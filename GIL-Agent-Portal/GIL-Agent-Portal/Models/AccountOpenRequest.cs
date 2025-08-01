public class AccountOpenRequest
{
    // nomineeDetails
    public string? nomineeName { get; set; }
    public string? nomineeDob { get; set; }
    public string? relationship { get; set; }
    public string? add1 { get; set; }
    public string? add2 { get; set; }
    public string? add3 { get; set; }
    public string? pin { get; set; }
    public string? nomineeState { get; set; }
    public string? nomineeCity { get; set; }

    // personalDetails
    public string? customername { get; set; }
    public string? customerLastName { get; set; }
    public string? dateofbirth { get; set; }
    public string? pincode { get; set; }
    public string? email { get; set; }
    public string? mobileNo { get; set; }

    // otherDeatils
    public string? maritalStatus { get; set; }
    public string? income { get; set; }
    public string? middleNameOfMother { get; set; }
    public string? houseOfFatherOrSpouse { get; set; }
    public string? kycFlag { get; set; }
    public string? panNo { get; set; }

    // additionalParameters
    public string? channelid { get; set; }
    public string? partnerid { get; set; }
    public string? applicationdocketnumber { get; set; }
    public string? dpid { get; set; }
    public string? clientid { get; set; }
    public string? tradingaccountnumber { get; set; }
    public string? partnerRefNumber { get; set; }
    public string? partnerpan { get; set; }
    public string? customerRefNumber { get; set; }
    public string? customerDematId { get; set; }
    public string? partnerCallBackURL { get; set; }
    public string? bcid { get; set; }
    public string? bcagentid { get; set; }

    // AgentId
    public string? AgentId { get; set; }

    public int? Status { get; set; } 
}
