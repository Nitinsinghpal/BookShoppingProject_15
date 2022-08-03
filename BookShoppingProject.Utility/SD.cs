using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShoppingProject.Utility
{
    public static class SD
    {
        //Roles

        public const String Role_Admin = "Admin";
        public const String Role_Employee = "Employee User";
        public const String Role_Company = "Company User";
        public const String Role_Individual = "Individual User";

        //sessions

        public const string Ss_Session = "Cart Count Session";

        public static double GetPriceBasedOnQuantity(double quantity,double price,double price50,double price100)
        {
            if (quantity < 50)
                return price;
            else if (quantity < 100)
                return price50;

            else
                return price100;    
        }

        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;
            for(int i =0; i<source.Length;i++)
            {
                char let = source[i];
                if(let == '<')
                {
                    inside = true;
                    continue;
                }
                if(let == '>')
                {
                    inside = false;
                    continue;
                }
                if(!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public const string Proc_CoverType_Create = "SP_CreateCoverType";
        public const string Proc_CoverType_Update = "SP_UpdateCoverType";
        public const string Proc_CoverType_Delete = "SP_DeleteCoverType";
        public const string GetCoverTypes = "SP_GetCoverTypes";
        public const string GetCoverType = "SP_GetCoverType";

        //orderStatus

        public const string OrderStatusPending = "Pending";
        public const string OrderStatusApproved = "Approved";
        public const string OrderStatusInProgress = "Processing";
        public const string OrderStatusShipping = "Shipped";
        public const string OrderStatusCancelled = "Cancelled";
        public const string OrderStatusRefund = "Refunded";

        //Payment Status

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayPayment = "PaymentStatusDelay";
        public const string PaymentStatusRejected = "Rejected";




    }
}
