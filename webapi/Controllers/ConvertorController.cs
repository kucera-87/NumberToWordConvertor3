using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using webapi.Comon;
using webapi.Model;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConvertorController : ControllerBase
    {

        [HttpPost(Name = "Convertor")]
        public ConvertorResponse Convert([FromBody] ConvertorRequest request)
        {
            ConvertorResponse response = new ConvertorResponse();
            StringBuilder sb = new StringBuilder();

            try
            {
                int teenNumber;
                double value;

                if (request == null)
                {
                    Logger.Log("Request is null");
                    return response;
                }

                if (String.IsNullOrEmpty(request.Input))
                {
                    response.Message = String.Empty;
                    return response;
                }

                string pattern = @"^(?:0|[1-9]\d{0,8})?(?:,\d{0,2})?$";

                if (!Regex.IsMatch(request.Input, pattern))
                {
                    response.Message = "The number is not in valid format.";
                    return response;
                }

                if (!Double.TryParse(request.Input, System.Globalization.NumberStyles.Number, CultureInfo.GetCultureInfo("de-DE"), out value))
                {
                    response.Message = "Input is not valid number.";
                    return response;
                }

                NumberIterator iterator = new NumberIterator((decimal)value);

                foreach (var numberData in iterator)
                {
                    try
                    {

                        if (numberData.DoSpace())
                            sb.Append(" ");

                        switch (numberData.GroupPosition)
                        {
                            case NumberData.GroupPositionEnum.Units:

                                if (numberData.IsTeenNumberCompleted(out teenNumber))
                                {
                                    sb.Append((Teens)teenNumber);
                                }
                                else
                                {
                                    if (numberData.DoPrintDash())
                                        sb.Append("-");

                                    if (numberData.Digit > 0 || numberData.IsZeroDollars())
                                        sb.Append((Units)numberData.Digit);
                                }

                                if (numberData.DoPrintGroupName())
                                {
                                    sb.Append(" ");

                                    if (numberData.Is1Dollar())
                                    {
                                        sb.Append(GroupName.dollar);
                                    }
                                    else if (numberData.Is1Cent())
                                    {
                                        sb.Append(GroupName.cent);
                                    }
                                    else
                                    {
                                        sb.Append((GroupName)numberData.GroupOfThousand);
                                    }

                                    if (numberData.DoPrintAnd())
                                        sb.Append(" and");
                                }

                                break;

                            case NumberData.GroupPositionEnum.Tens:

                                if (numberData.IsTeenNumber()) continue;

                                if (numberData.Digit > 0)
                                    sb.Append((Tens)(numberData.Digit * 10));

                                break;

                            case NumberData.GroupPositionEnum.Hundreds:

                                if (numberData.Digit > 0)
                                    sb.AppendFormat("{0} {1}", (Units)numberData.Digit, "hundert");

                                break;
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.Error(String.Format("Input value: {0}; Current digit: {1}; Group name: {2}, Group position: {3}, Error message: {4}",
                            request.Input, numberData.Digit, numberData.GroupOfThousand, numberData.GroupPosition, ex.Message));

                        break;
                    }
                }
                response.Output = sb.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error(String.Format("Input value: {0}; Error message: {1}", request.Input, ex.Message));
            }

            return response;
        }
    }
}
