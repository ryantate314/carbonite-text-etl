//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MessageImport.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class MessageAttachment
    {
        public long Key { get; set; }
        public long MessageKey { get; set; }
        public long AttachmentKey { get; set; }
    
        public virtual Attachment Attachment { get; set; }
        public virtual Message Message { get; set; }
    }
}
