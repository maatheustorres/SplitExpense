using SplitExpense.Domain.Core.Primitives;

namespace SplitExpense.Domain.Core.Errors;

public static class DomainErrors
{
    public static class User
    {
        public static readonly Error DuplicateEmail = new("User.DuplicateEmail", "The specified email is already in use.");
        public static readonly Error NotFound = new("User.NotFound", "The user with the specified identifier was not found.");
    }

    public static class Name
    {
        public static Error NullOrEmpty => new("Name.NullOrEmpty", "The name is required");
        public static Error LongerThanAllowed => new("Name.LongerThanAllowed", "The name is longer than allowed.");
    }

    public static class FirstName
    {
        public static Error NullOrEmpty => new("FirstName.NullOrEmpty", "The first name is required");
        public static Error LongerThanAllowed => new("FirstName.LongerThanAllowed", "The first name is longer than allowed.");
    }
    
    public static class LastName
    {
        public static Error NullOrEmpty => new("LastName.NullOrEmpty", "The last name is required");
        public static Error LongerThanAllowed => new("LastName.LongerThanAllowed", "The last name is longer than allowed.");
    }

    public static class Email
    {
        public static Error NullOrEmpty => new("Email.NullOrEmpty", "The email is required.");
        public static Error LongerThanAllowed => new("Email.LongerThanAllowed", "The email is longer than allowed.");
        public static Error InvalidFormat => new("Email.InvalidFormat", "The email format is invalid.");
    }

    public static class Password
    {
        public static Error NullOrEmpty => new("Password.NullOrEmpty", "The password is required.");

        public static Error TooShort => new("Password.TooShort", "The password is too short.");

        public static Error MissingUppercaseLetter => new(
            "Password.MissingUppercaseLetter",
            "The password requires at least one uppercase letter.");

        public static Error MissingLowercaseLetter => new(
            "Password.MissingLowercaseLetter",
            "The password requires at least one lowercase letter.");

        public static Error MissingDigit => new(
            "Password.MissingDigit",
            "The password requires at least one digit.");

        public static Error MissingNonAlphaNumeric => new(
            "Password.MissingNonAlphaNumeric",
            "The password requires at least one non-alphanumeric.");
    }

    public static class Authentication
    {
        public static Error InvalidEmailOrPassword => new(
            "Authentication.InvalidEmailOrPassword",
            "The specified email or password are incorrect.");
    }

    public static class Category
    {
        public static Error NotFound => new("Category.NotFound", "The category with the specified identifier was not found.");
    }

    public static class Group
    {
        public static Error NotFound => new("Group.NotFound", "The group with the specified identifier was not found.");
        public static Error AlreadyAdded => new("Group.AlreadyAdded", "The user(s) has already been added");
        public static Error NoUser => new("Group.NoUser", "No user has been added to the group");
    }
}
