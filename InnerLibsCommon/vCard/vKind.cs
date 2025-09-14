using System.ComponentModel.DataAnnotations;

namespace Extensions.vCards
{
    /// <summary>
    /// Tipo de entidade vCard (KIND) - novo no vCard 4.0
    /// </summary>
    public enum vKind
    {
        [Display(Name = "Individual")]
        Individual,  // Pessoa individual (padrão)
        [Display(Name = "Group")]
        Group,       // Grupo de contatos
        [Display(Name = "Organization")]
        Org,         // Organização
        [Display(Name = "Location")]
        Location     // Localização
    }
}