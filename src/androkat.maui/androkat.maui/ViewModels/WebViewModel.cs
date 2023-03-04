﻿using androkat.hu.Models;
using androkat.hu.Services;

namespace androkat.hu.ViewModels;

public partial class WebViewModel : ViewModelBase
{
    private readonly PageService _pageService;

    public WebViewModel(PageService pageService)
    {
        _pageService = pageService;

        //dialog title: Katolikus weboldalak

        WebPageUrls.Add(new WebUrl("Katekizmus", "https://archiv.katolikus.hu/kek/"));
        WebPageUrls.Add(new WebUrl("E-Biblia", "http://szentiras.hu/"));
        WebPageUrls.Add(new WebUrl("Zsolozsma", "http://zsolozsma.katolikus.hu/"));
        WebPageUrls.Add(new WebUrl("MiseRend", "http://miserend.hu/"));
        WebPageUrls.Add(new WebUrl("Megszentelt tér", "http://www.szentter.com/"));
        WebPageUrls.Add(new WebUrl("Bonum TV élő", "https://katolikus.tv/elo-adas/"));
        WebPageUrls.Add(new WebUrl("liturgia.tv", "https://liturgia.tv/"));        
    }

    public List<WebUrl> WebPageUrls = new();
}
