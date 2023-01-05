﻿using androkat.application.Interfaces;
using androkat.domain.Enum;
using androkat.infrastructure.DataManager;
using androkat.infrastructure.Mapper;
using androkat.infrastructure.Model.SQLite;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Linq;

namespace androkat.infrastructure.Tests;

public class CacheRepositoryTests : BaseTest
{
    [Test]
    public void GetHumorToCache_Happy()
    {
        var logger = new Mock<ILogger<CacheRepository>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        var mapper = config.CreateMapper();

        var clock = GetToday();

        using (var context = new AndrokatContext(GetDbContextOptions()))
        {
            var entity = new FixContent
            {
                Datum = "02-03",
                Tipus = (int)Forras.humor
            };
            context.FixContent.Add(entity);
            context.SaveChanges();

            var repo = new CacheRepository(context, logger.Object, clock.Object, mapper);
            var result = repo.GetHumorToCache();
            result.First().Fulldatum.ToString("yyyy-MM-dd").Should().Be(DateTime.Now.ToString("yyyy-") + entity.Datum);
        }
    }

    [TestCase((int)Forras.humor)]
    [TestCase((int)Forras.pio)]
    public void GetNapiFixToCache_Happy(int tipus)
    {
        var logger = new Mock<ILogger<CacheRepository>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        var mapper = config.CreateMapper();

        var clock = GetToday();

        using (var context = new AndrokatContext(GetDbContextOptions()))
        {
            var entity = new FixContent
            {
                Datum = "02-03",
                Tipus = tipus
            };
            context.FixContent.Add(entity);
            context.SaveChanges();

            var repo = new CacheRepository(context, logger.Object, clock.Object, mapper);
            var result = repo.GetNapiFixToCache();
            if (tipus == (int)Forras.pio)
            {
                result.Count().Should().Be(1);  
                result.First().Fulldatum.ToString("yyyy-MM-dd").Should().Be(DateTime.Now.ToString("yyyy-") + entity.Datum);
            }
            else
            {
                result.Count().Should().Be(0);
            }
        }
    }

    [Test]
    public void GetMaiSzentToCache_Ma_Happy()
    {
        var logger = new Mock<ILogger<CacheRepository>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        var mapper = config.CreateMapper();

        var clock = GetToday();

        using (var context = new AndrokatContext(GetDbContextOptions()))
        {
            var entity = new Maiszent
            {
                Datum = "02-03"
            };
            context.MaiSzent.Add(entity);
            context.SaveChanges();

            var repo = new CacheRepository(context, logger.Object, clock.Object, mapper);
            var result = repo.GetMaiSzentToCache();
            result.First().Fulldatum.ToString("yyyy-MM-dd").Should().Be(DateTime.Now.ToString("yyyy-") + entity.Datum);
        }
    }

    [Test]
    public void GetMaiSzentToCache_Tegnap_Happy()
    {
        var logger = new Mock<ILogger<CacheRepository>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        var mapper = config.CreateMapper();

        var clock = GetToday();

        using (var context = new AndrokatContext(GetDbContextOptions()))
        {
            var entity = new Maiszent
            {
                Datum = "02-02"
            };
            context.MaiSzent.Add(entity);
            context.SaveChanges();

            var repo = new CacheRepository(context, logger.Object, clock.Object, mapper);
            var result = repo.GetMaiSzentToCache();
            result.First().Fulldatum.ToString("yyyy-MM-dd").Should().Be(DateTime.Now.ToString("yyyy-") + entity.Datum);
        }
    }

    [Test]
    public void GetMaiSzentToCache_ElozoHonap_Happy()
    {
        var logger = new Mock<ILogger<CacheRepository>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        var mapper = config.CreateMapper();

        var clock = GetToday();

        using (var context = new AndrokatContext(GetDbContextOptions()))
        {
            var entity = new Maiszent
            {
                Datum = "01-31"
            };
            context.MaiSzent.Add(entity);
            context.SaveChanges();

            var repo = new CacheRepository(context, logger.Object, clock.Object, mapper);
            var result = repo.GetMaiSzentToCache();
            result.First().Fulldatum.ToString("yyyy-MM-dd").Should().Be(DateTime.Now.ToString("yyyy-") + entity.Datum);
        }
    }

    private static Mock<IClock> GetToday()
    {
        var clock = new Mock<IClock>();
        clock.Setup(c => c.Now).Returns(DateTimeOffset.Parse(DateTime.Now.ToString("yyyy") + "-02-03T04:05:06"));
        return clock;
    }

    [Test]
    public void GetContentDetailsModelToCache_Happy()
    {
        var logger = new Mock<ILogger<CacheRepository>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        var mapper = config.CreateMapper();

        var clock = GetToday();

        using (var context = new AndrokatContext(GetDbContextOptions()))
        {
            var entity = new Napiolvaso
            {
                Fulldatum = DateTime.Now.ToString("yyyy") + "-02-03",
                Tipus = (int)Forras.audiohorvath
            };
            context.Content.Add(entity);
            context.SaveChanges();

            var repo = new CacheRepository(context, logger.Object, clock.Object, mapper);
            var result = repo.GetContentDetailsModelToCache();
            result.First().Fulldatum.ToString("yyyy-MM-dd").Should().Be(DateTime.Now.ToString("yyyy-02-03"));
        }        
    }
}