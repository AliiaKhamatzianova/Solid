using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Course
{
    // Класс, содержащий методы расширения
    public static class Extensions
    {
        // Метод, отсутствующий в интерфейсе IQuack
        public static void Done(this IQuack quacker)
        {
            Console.WriteLine("Done quack");
        }

        // Метод, присутствующий в интерфейсе IQuack.
        // У интерфейса за счёт этого метода появляется реализация Quack "по-умолчанию".
        public static void Quack(this IQuack quacker)
        {
            Console.WriteLine("Quack");
        }

        // Метод, присутствующий в интерфейсе IQuack.
        // У интерфейса за счёт этого метода появляется реализация Ready "по-умолчанию".
        public static void Ready(this IQuack quacker)
        {
            Console.WriteLine("Ready to quack...");
        }
    }

    // Интерфейс "крякающих" объектов.
    public interface IQuack
    {
        // Крякать.
        void Quack();

        // Готовность крякать.
        void Ready();
    }

    // Интерфейс "летающих" объектов.
    public interface IFly
    {
        // Летать.
        void Fly();

        // Готовность к полёту.
        void Ready();
    }

    // Класс утки - летающего и крякающего существа.
    public class Duck : IQuack, IFly
    {
        // Реализация крякания в классе.
        public void Quack()
        {
            Console.WriteLine("Duck quack!");
        }

        // Реализация полёта в классе.
        public void Fly()
        {
            Console.WriteLine("Duck flies!");
        }

        // Реализация готовности к полёту и кряканию в классе.
        // При наличии этого метода он будет в приоритете
        // перед методом расширения и явной реализации метода.
        //public void Ready()
        //{
        //    Console.WriteLine("Ready to quack and fly");
        //}

        // Явная реализация готовности крякать для интерфейса IQuack.
        // Срабатывает при обращении по интерфейсной ссылке.
        void IQuack.Ready()
        {
            Console.WriteLine("Ready to quack!");
        }

        // Явная реализация готовности крякать для интерфейса IFly.
        // Срабатывает при обращении по интерфейсной ссылке.
        void IFly.Ready()
        {
            Console.WriteLine("Ready to fly");
        }
    }

    // Класс игрушечной утки, способной крякать
    public class ToyDuck : IQuack
    {
        // Явная реализация готовности крякать для интерфейса IQuack.
        // Срабатывает при обращении по интерфейсной ссылке.
        void IQuack.Quack()
        {
            Console.WriteLine("Toy quack!");
        }

        // Явная реализация готовности крякать для интерфейса IFly.
        // Срабатывает при обращении по интерфейсной ссылке.
        void IQuack.Ready()
        {
            Console.WriteLine("Ready to toy quack!");
        }
    }

    // Интерфейс с контравариантным параметром типа
    // Тип контравариантного параметра может находиться
    // только во входной позиции (input) - в параметрах метода.
    // Нельзя использовать контравариантные типы в выходных позициях.
    public interface IProcessor<in T>
    {
        void Process(IEnumerable<T> items);
    }

    // Реализация интерфейса, реализующего возможность обработки элементов.
    public class Processor<T> : IProcessor<T> where T : IQuack
    {
        public void Process(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                item.Quack();
            }
        }
    }

    // Интерфейс с ковариантным параметром типа
    // Внимание, ковариантный параметр может находиться
    // только в выходной позиции (output position) - возвращаемом значении
    // Этот тип нельзя использовать во входной позиции
    public interface IFactory<out T>
    {
        T CreateInstance();
    }

    // Класс - "Фабрика", реализующий возможность создания объектов разных типов.
    public class Factory<T> : IFactory<T> where T : new()
    {
        public T CreateInstance()
        {
            return new T();
        }
    }

    class Dependent
    {
        private IDependency _dependency;

        public Dependent (IDependency d)
        {
            _dependency = d;
        }

        public void Do()
        {
            _dependency.Foo("Hello");
            Console.WriteLine("!!!");
        }
    }

    interface IDependency
    {
        void Foo(string str);
    }

    class Dependency:IDependency
    {
        public void Foo(string str)
        {
            Console.WriteLine(str);
        }
    }

    class DependencyFile:IDependency
    {
        public void Foo(string str)
        {
            using (var sw = new StreamWriter("output.txt"))
            {
                sw.Write(str);
            }
                
        }
    }

    class Program
    {
        // Метод, подходящий для делегата Action<object>.
        static void Foo(object o)
        {
            Console.WriteLine("Hello world");
        }

        // Точка входа в программу.
        static void Main()
        {
            var d = new Dependent(new Dependency());

            Stream s = new FileStream("output.txt", FileMode.Append);

            using (var sw = new StreamWriter(s))
            {

            }
            /*
            // Списки уток и игрушечных уток.
            var duckList = new List<Duck> { new Duck() };
            var toyDuckList = new List<ToyDuck> { new ToyDuck() };

            // Обработчик коллекций квакающих объектов. Работа через интерфейсную ссылку на интерфейс общего типа.
            // В параметре обобщенной интерфейсой ссылкй указывается более "широкий" тип, чем в конкретном обработчике.
            IProcessor<IQuack> quackProcessor = new Processor<IQuack>();
            // Обработка коллекции квакающих объектов.
            // До .NET4 нужно было писать так:
            //proc.Process(duckList.OfType<IQuack>());
            // С .NET4 и ковариацией IEnumerable<out T> стало возможно писать проще:
            quackProcessor.Process(duckList);

            // Пример использования пользовательских ковариантных типов.
            // Фабрики для создания крякающих объектов.
            // Работаем с фабриками через интерфейсные ссылки.
            // Тип в параметре интерфейсной ссылки более общий, чем в конкретных фабриках.
            IFactory<IQuack> duckFactory = new Factory<Duck>();
            IFactory<IQuack> toyDuckFactory = new Factory<ToyDuck>();
            // Объединение нескольких фабрик крякающих объектво в один список по интерфейсным ссылкам.
            List<IFactory<IQuack>> factories = new List<IFactory<IQuack>> { duckFactory, toyDuckFactory };
            // Создание крякающих объектов с помощью различных фабрик из списка.
            foreach (var factory in factories)
            {
                factory.CreateInstance().Ready();
            }

            // Пример использования пользовательского контравариантного типа.
            // Работаем с обработчиками крякающих объектов через интерфейсные ссылки.
            // Параметр типа в интерфейсной ссылке более "узкий", чем тип "конкретных" обработчиков, 
            // передаваемых по интерфейсной ссылке на крякающий объект.
            IProcessor<Duck> duckProcessor = quackProcessor;
            IProcessor<ToyDuck> toyDuckProcessor = quackProcessor;
            // Типобезопасное использование контраватиантных интерфейсных ссылок.
            // Можем ипользовать обработчик уток только для коллекций уток.
            duckProcessor.Process(duckList);
            toyDuckProcessor.Process(toyDuckList);


            // Работа с объектом через ссылку на объект.
            Duck duck1 = new Duck();
            // При работе с объектом через ссылку на объект 
            // при отсутствии метода Ready в классе и наличии 
            // метода расширения и явной реализации интерфейса
            // будет вызван метод расширения!
            duck1.Ready();

            // Работа с объектом через интерфейсную ссылку.
            IQuack quack1 = new Duck();
            // При работе с объектом через интерфейсную ссылку
            // при отсутствии метода Ready в классе и наличии 
            // метода расширения и явной реализации интерфейса
            // будет вызвана явная реализация интерфейса!
            quack1.Ready();
            // Вызов метода, отсутствующего в интерфейсе, 
            // но описанного как метод расширения.
            quack1.Done();
            */
        }
    }
}