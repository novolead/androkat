using androkat.application.Interfaces;
using androkat.domain.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace androkat.web.Pages;

public class HanganyagModel : PageModel
{
    private readonly IContentService _contentService;

    public HanganyagModel(IContentService contentService)
    {
        _contentService = contentService;
    }

    public IReadOnlyCollection<AudioViewModel> AudioViewModels { get; set; }

    public void OnGet()
    {
        AudioViewModels = _contentService.GetAudio().OrderBy(o => o.Tipus).ToList();
    }
}