﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MNIT_Communication.Domain;
using StackExchange.Redis.KeyspaceIsolation;

namespace MNIT_Communication.Services.Fakes
{
    public class FakeRepository: IRepository
    {
        private Dictionary<Guid, Entry> data = new Dictionary<Guid, Entry>();

        public async Task<IList<T>> Get<T>() where T : BaseEntity
        {
            return await Task.Run(() =>
            {
                var typeMatches = data.Values.Where(e => e.Type == typeof (T));
                var list = typeMatches.Cast<Entry<T>>().Select(e => e.Value).ToList();
                return list;
            });
        }

        public async Task<T> Get<T>(Guid id) where T : BaseEntity
        {
            if (!(await Exists(id)))
            {
                return default(T);
            }

            var match = data[id] as Entry<T>;
            
            if (match == null)
            {
                return default(T);
            }

            return match.Value;
        }

        public async Task<IList<T>> Get<T>(Expression<Func<T, bool>> predicate) where T : BaseEntity
        {
            var list = await Get<T>();
            return list.Where(predicate.Compile()).ToList();
        }

        public async Task<T> Upsert<T>(T item) where T: BaseEntity
        {
            if(await Exists(item.Id))
            {
                data.Remove(item.Id);
            }

            var entry = new Entry<T>(item);

            data.Add(item.Id, entry);

            return await Get<T>(item.Id);
        }
        public async Task Delete<T>(T item) where T : BaseEntity
        {
            await Task.Run(() => data.Remove(item.Id));
        }

        public async Task Delete<T>(Guid id) where T : BaseEntity
        {
            await Task.Run(() => data.Remove(id));
        }

        private async Task<bool> Exists(Guid id)
        {
            return await Task.Run(() => data.ContainsKey(id));
        }

        private abstract class Entry
        {
            public abstract Type Type { get; }
        } 
        
        private class Entry<T>: Entry
        {
            public Entry(T value)
            {
                Value = value;
            }

            public T Value { get; set; }

            public override Type Type
            {
                get { return typeof (T); }
            }
        }
    }


}
