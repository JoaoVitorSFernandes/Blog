using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Categories
{
    /*Esse ViewModel foi criado pois a entrada de dados na 
    criação de uma category não correspondia com os valores 
    do proprio model da categoria, por exemplo, o model possui
    Posts e Id, porém o usuario não interage com esses campos 
    na criação de uma nova category o que estava deixando o codigo 
    de certa forma vamos dizer desnecessario.*/
    public class EditorCategoryViewModel
    {
        [Required(ErrorMessage = "O nome e obrigatório")]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "Esse campo deve ter entre 3 e 40 caractereres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O Slug e obrigatório")]
        public string Slug { get; set; }
    }
}