using System;
using System.Collections;
using System.Collections.Generic;

namespace backend.CityModel
{
    public class BaseModel
    {
        public List<Entities.Stop> Stops = new List<Entities.Stop>();
        public List<Entities.Route> Routes = new List<Entities.Route>();

        public BaseModel() {}

        public T CloneAs<T>() where T: BaseModel, new()
        {
            T instance = new T();
            instance.Stops = new List<Entities.Stop>(this.Stops);
            instance.Routes = new List<Entities.Route>(this.Routes);
            return instance;
        }
    }
}