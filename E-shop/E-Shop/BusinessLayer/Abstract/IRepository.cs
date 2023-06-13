﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IRepository<T>where T:class,new()// T class özelliğinde ve aynı zamanda yenilenebilir olacak.
    {
        List<T> List();
        void Insert(T p);
        void Delete(T p);
        void Update(T p);
        T GetById(int id);
    }
}
