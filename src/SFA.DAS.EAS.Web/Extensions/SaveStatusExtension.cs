using SFA.DAS.EAS.Web.Enums;

namespace SFA.DAS.EAS.Web.Extensions
{
    public static class SaveStatusExtension
     {
         public static bool IsSend(this SaveStatus status)
         {
             return status == SaveStatus.ApproveAndSend || status == SaveStatus.AmendAndSend;
         }
 
         public static bool IsApproveWithoutSend(this SaveStatus status)
         {
             return status == SaveStatus.Approve;
         }
     }
}