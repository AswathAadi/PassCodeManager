using PassCodeManager.DTO.RequestObjects;

namespace PassCodeManager.DTO.ResponseObjects
{
    public class SecurityResponseObject : AddPasscodeObject
    {
        public string Id { get;set; }
        public bool IsActive { get;set; }
    }
}
