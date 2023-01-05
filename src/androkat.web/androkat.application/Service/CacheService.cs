﻿using androkat.application.Interfaces;
using androkat.domain;
using androkat.domain.Model;
using androkat.domain.Model.ContentCache;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace androkat.application.Service;

public class CacheService : ICacheService
{
    private readonly ICacheRepository _cacheRepository;
    private readonly ILogger<CacheService> _logger;
    protected readonly IClock _clock;

    public CacheService(ICacheRepository cacheRepository, ILogger<CacheService> logger, IClock clock)
    {
        _cacheRepository = cacheRepository;
        _logger = logger;
        _clock = clock;
    }

    public MainCache MainCacheFillUp()
    {
        _logger.LogInformation("{name} was called", nameof(MainCacheFillUp));
        try
        {
            var result = new List<ContentDetailsModel>();
            AddHumorToCache(result);
            AddMaiSzentToCache(result);
            AddNapiFixToCache(result);
            result.AddRange(_cacheRepository.GetContentDetailsModelToCache());

            return new MainCache
            {
                ContentDetailsModels = result,
                Inserted = _clock.Now.DateTime
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception: {name}", nameof(MainCacheFillUp));
        }

        return new MainCache
        {
            ContentDetailsModels = new List<ContentDetailsModel>(),
            Inserted = _clock.Now.DateTime
        };
    }

    private void AddNapiFixToCache(List<ContentDetailsModel> result)
    {
        result.AddRange(_cacheRepository.GetNapiFixToCache());
    }

    private void AddHumorToCache(List<ContentDetailsModel> result)
    {
        var humor = _cacheRepository.GetHumorToCache();
        foreach (var item in humor)
        {
            result.Add(item);
        }
    }

    private void AddMaiSzentToCache(List<ContentDetailsModel> result)
    {
        var szent = _cacheRepository.GetMaiSzentToCache();
        foreach (var item in szent)
        {
            result.Add(item);
        }
    }
}