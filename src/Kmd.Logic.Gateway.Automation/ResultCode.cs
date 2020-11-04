namespace Kmd.Logic.Gateway.Automation
{
    public enum ResultCode
    {
        /// <summary>
        /// Invalid input data.
        /// </summary>
        InvalidInput,

        /// <summary>
        /// Product created successfully.
        /// </summary>
        ProductCreated,

        /// <summary>
        /// Input validation succeeded.
        /// </summary>
        PublishingValidationSuccess,

        /// <summary>
        /// Input validation failed.
        /// </summary>
        PublishingValidationFailed,

        /// <summary>
        /// Product validated successfully.
        /// </summary>
        ProductValidated,
    }
}
