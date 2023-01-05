﻿using androkat.application.Interfaces;
using androkat.domain;
using androkat.domain.Configuration;
using androkat.domain.Enum;
using androkat.domain.Model;
using androkat.infrastructure.Model.SQLite;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace androkat.infrastructure.DataManager;

public class CacheRepository : BaseRepository, ICacheRepository
{
    private readonly ILogger<CacheRepository> _logger;

    public CacheRepository(AndrokatContext ctx,
        ILogger<CacheRepository> logger,
        IClock clock,
        IMapper mapper) : base(ctx, clock, mapper)
    {
        _logger = logger;
    }

    public IEnumerable<ContentDetailsModel> GetHumorToCache()
    {
        var list = new List<ContentDetailsModel>();

        var month = _clock.Now.ToString("MM");
        var rows = _ctx.FixContent.AsNoTracking().AsEnumerable()
            .Where(w => w.Tipus == (int)Forras.humor && w.Datum.StartsWith($"{month}-") && w.FullDate < _clock.Now)
            .OrderByDescending(o => o.Datum).ToList();

        rows.ForEach(w =>
        {
            w.Datum = _clock.Now.ToString("yyyy-") + w.Datum + " 00:00:01";
            list.Add(_mapper.Map<ContentDetailsModel>(w));
        });

        return _mapper.Map<List<ContentDetailsModel>>(list);
    }

    public IEnumerable<ContentDetailsModel> GetMaiSzentToCache()
    {
        var list = new List<ContentDetailsModel>();
        var hoNap = _clock.Now.ToString("MM-dd");
        var month = _clock.Now.ToString("MM");

        var rows = _ctx.MaiSzent.AsNoTracking().Where(w => w.Datum == hoNap);
        if (rows.Any())
        {
            rows.ToList().ForEach(row =>
            {
                row.Datum = _clock.Now.ToString("yyyy-") + row.Datum;
                list.Add(_mapper.Map<ContentDetailsModel>(row));
            });

            return list;
        }

        var rows2 = _ctx.MaiSzent.AsNoTracking().AsEnumerable()
        .Where(w => w.Datum.StartsWith($"{month}-") && w.FullDate < _clock.Now)
        .OrderByDescending(o => o.Datum).Take(1);

        if (!rows2.Any())
        {
            var prevmonth = _clock.Now.AddMonths(-1).ToString("MM");
            //nincs az új hónap első napján anyag
            rows2 = _ctx.MaiSzent.AsNoTracking().AsEnumerable()
                .Where(w => w.Datum.StartsWith($"{prevmonth}-") && w.FullDate < _clock.Now)
                .OrderByDescending(o => o.Datum).Take(1);
        }

        rows2.ToList().ForEach(row =>
        {
            row.Datum = _clock.Now.ToString("yyyy-") + row.Datum;
            list.Add(_mapper.Map<ContentDetailsModel>(row));
        });

        return list;
    }

    public IEnumerable<ContentDetailsModel> GetNapiFixToCache()
    {
        var result = new List<ContentDetailsModel>();

        var tipusok = new List<int>();
        AndrokatConfiguration.FixContentTypeIds().Where(w => w != (int)Forras.humor).ToList().ForEach(f =>
        {
            tipusok.Add(f);
        });

        var date = _clock.Now.ToString("MM-dd");

        var napiFixek = _ctx.FixContent.AsNoTracking().Where(w => tipusok.Contains(w.Tipus) && w.Datum == date);
        if (napiFixek == null)
            return result;

        foreach (var napiFix in napiFixek)
        {
            napiFix.Datum = _clock.Now.ToString("yyyy-") + napiFix.Datum + " 00:00:01";
            result.Add(_mapper.Map<ContentDetailsModel>(napiFix));
        }

        return result;
    }

    public IEnumerable<ContentDetailsModel> GetContentDetailsModelToCache()
    {
        var result = new List<ContentDetailsModel>();

        var tipusok = AndrokatConfiguration.ContentTypeIds();

        //ezekből az összes elérhető kell, nem csak az adott napi
        var osszes = new List<int>
        {
            (int)Forras.audiohorvath, (int)Forras.audiotaize,
            (int)Forras.audiobarsi, (int)Forras.audionapievangelium,
            (int)Forras.ajanlatweb, (int)Forras.audiopalferi,
            (int)Forras.prayasyougo
        };

        var tomorrow = _clock.Now.AddDays(1).ToString("yyyy-MM-dd");

        foreach (var tipus in tipusok)
        {
            var date = _clock.Now.ToString("yyyy-MM-dd");

            if (tipus == (int)Forras.fokolare)
                date = _clock.Now.ToString("yyyy-MM") + "-01";

            IQueryable<Napiolvaso> res = GetRes(tipus, date, tomorrow, osszes);

            result.AddRange(_mapper.Map<List<ContentDetailsModel>>(res));
        }

        return result;
    }

    private IQueryable<Napiolvaso> GetRes(int tipus, string date, string tomorrow, List<int> osszes)
    {
        IQueryable<Napiolvaso> res;
        if (tipus == (int)Forras.maievangelium) //szombaton már megjelenik a vasárnapi is
            res = _ctx.Content.AsNoTracking().Where(w => w.Tipus == tipus && (w.Fulldatum.StartsWith(date) || w.Fulldatum.StartsWith(tomorrow))).OrderByDescending(o => o.Inserted);
        else if (osszes.Contains(tipus)) //ajanlo és néhány hanganyagból a weboldalon látszik mindegyik 
            res = _ctx.Content.AsNoTracking().Where(w => w.Tipus == tipus).OrderByDescending(o => o.Inserted);
        else
            res = _ctx.Content.AsNoTracking().Where(w => w.Tipus == tipus && w.Fulldatum.StartsWith(date)).OrderByDescending(o => o.Inserted);

        //ha nincs mai, akkor egy a korábbiakból, ha van
        if (res == null || !res.Any())
            res = _ctx.Content.AsNoTracking().Where(w => w.Tipus == tipus).OrderByDescending(o => o.Inserted).Take(1);
        return res;
    }
}