using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Template.Application.Interfaces.Models
{
    public interface IComponentPaymentCancellationReturn
    {
        /// <summary>
        /// Gets or sets the component token.
        /// </summary>
        /// <value>
        /// The component token.
        /// </value>
        int ComponentToken { get; set; }

        /// <summary>
        /// Gets or sets the payments.
        /// </summary>
        /// <value>
        /// The payments.
        /// </value>
        List<IPayment> Payments { get; set; }

        /// <summary>
        /// Gets or sets the cancellation charges.
        /// </summary>
        /// <value>
        /// The cancellation charges.
        /// </value>
        List<ICancellationCharge> CancellationCharges { get; set; }

        /// <summary>
        /// Gets or sets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        List<string> Warnings { get; set; }
    }
}
