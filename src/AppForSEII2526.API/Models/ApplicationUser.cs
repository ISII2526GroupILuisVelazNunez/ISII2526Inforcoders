using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser {
    public ApplicationUser() { //For SeedData
        PaymentMethods = new List<PaymentMethod>();
        Incidents = new List<Incident>();
    }
    public ApplicationUser(string name, string surname)
        : this()  // in order to also initialize lists when using this constructor
    {
        Name = name;
        Surname = surname;
    }

    public ApplicationUser(string name, string surname, IList<PaymentMethod> paymentMethods, IList<Incident> incidents)
    {
        Name = name;
        Surname = surname;
        PaymentMethods = paymentMethods;
        Incidents = incidents;
    }

    public string Name { get; set; }
    public string Surname { get; set; }
    public IList<PaymentMethod> PaymentMethods { get; set; }
    public IList<Incident> Incidents { get; set; }
}