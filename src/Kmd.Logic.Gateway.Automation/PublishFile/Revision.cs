namespace Kmd.Logic.Gateway.Automation.PublishFile
{
    internal class Revision
    {
        public string RevisionDescription { get; set; }

        public string OpenApiSpecFile { get; set; }

        public bool IsCurrent { get; set; }
    }
}