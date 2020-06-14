using ModernPlayerManagementAPI.Services;
using Xunit;

namespace ModernPlayerManagementAPITests
{
    public class EmailValidatorTest
    {
        [Fact]
        void Valid_Email_Return_True()
        {
            // Given 
            var email = "arsene@lapostolet.fr";
            
            // When
            var result = new EmailValidator().IsValidEmail(email);
            
            // Then
            Assert.True(result);
        }
        
        [Fact]
        void Valid_Email_Return_False()
        {
            // Given 
            var email = "arsenelapostolet.fr";
            
            // When
            var result = new EmailValidator().IsValidEmail(email);
            
            // Then
            Assert.False(result);
        }
    }
}