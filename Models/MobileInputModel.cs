namespace Menu4Tech.Models;

public class MobileInputModel
{
    public string Name { get; }
    public string SelectedCountryIso { get; set; }
    public string InitialMobile { get; set; }

    public MobileInputModel(string name)
    {
        Name = name;
    }
    public MobileInputModel(string name, string selectedCountryIso)
    {
        Name = name;
        SelectedCountryIso = selectedCountryIso;
    }
    
    public MobileInputModel(string name, string selectedCountryIso, string initialMobile)
    {
        Name = name;
        SelectedCountryIso = selectedCountryIso;
        InitialMobile = initialMobile;
    }

}