using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CarboniteXmlParser.Test
{
   [TestClass]
   public class SMSTest
   {

      MessageReader reader;

      [TestInitialize]
      public void Setup()
      {
         reader = new MessageReader("TestMessages.xml");
      }

     
   }
}
