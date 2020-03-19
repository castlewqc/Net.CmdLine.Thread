using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Net.CmdLine.ThreadStudy.反射
{
    public delegate void D1(C c, string s);
    public delegate void D2(string s);
    public delegate void D3();
    class DelegateSample
    {
        public static void Main()
        {
            C c1 = new C(42);

            // Get a MethodInfo for each method.
            //
            MethodInfo mi1 = typeof(C).GetMethod("M1",
                BindingFlags.Public | BindingFlags.Instance);
            MethodInfo mi2 = typeof(C).GetMethod("M2",
                BindingFlags.Public | BindingFlags.Static);

            D1 d1;
            D2 d2;
            D3 d3;


            Console.WriteLine("\nAn instance method closed over C.");
            // In this case, the delegate and the
            // method must have the same list of argument types; use
            // delegate type D2 with instance method M1.
            //
            Delegate test =
                Delegate.CreateDelegate(typeof(D2), c1, mi1, false);

            // Because false was specified for throwOnBindFailure 
            // in the call to CreateDelegate, the variable 'test'
            // contains null if the method fails to bind (for 
            // example, if mi1 happened to represent a method of  
            // some class other than C).
            //
            if (test != null)
            {
                d2 = (D2)test;

                // The same instance of C is used every time the 
                // delegate is invoked.
                d2("Hello, World!");
                d2("Hi, Mom!");
            }


            Console.WriteLine("\nAn open instance method.");
            // In this case, the delegate has one more 
            // argument than the instance method; this argument comes
            // at the beginning, and represents the hidden instance
            // argument of the instance method. Use delegate type D1
            // with instance method M1.
            //
            d1 = (D1)Delegate.CreateDelegate(typeof(D1), null, mi1);

            // An instance of C must be passed in each time the 
            // delegate is invoked.
            //
            d1(c1, "Hello, World!");
            d1(new C(5280), "Hi, Mom!");


            Console.WriteLine("\nAn open static method.");
            // In this case, the delegate and the method must 
            // have the same list of argument types; use delegate type
            // D2 with static method M2.
            //
            d2 = (D2)Delegate.CreateDelegate(typeof(D2), null, mi2);

            // No instances of C are involved, because this is a static
            // method. 
            //
            d2("Hello, World!");
            d2("Hi, Mom!");


            Console.WriteLine("\nA static method closed over the first argument (String).");
            // The delegate must omit(忽略) the first argument of the method.
            // A string is passed as the firstArgument parameter, and 
            // the delegate is bound to(绑定) this string. Use delegate type 
            // D3 with static method M2. 
            //
            d3 = (D3)Delegate.CreateDelegate(typeof(D3),
                "Hello, World!", mi2);

            // Each time the delegate is invoked, the same string is
            // used.
            d3();


            //var d4 = (D3)Delegate.CreateDelegate(typeof(D3), null, mi2);
            //d4("312"); --d3未采用1个参数
        }
    }
    public class C
    {
        private int id;
        public C(int id) { this.id = id; }

        public void M1(string s)
        {
            Console.WriteLine("Instance method M1 on C:  id = {0}, s = {1}",
                this.id, s);
        }

        public static void M2(string s)
        {
            Console.WriteLine("Static method M2 on C:  s = {0}", s);
        }
    }
    public class Base { }

    public class Derived : Base
    {
        // Define a static method to use in the demonstration(示范). The method 
        // takes an instance of Base and returns an instance of Derived.  
        // For the purposes of the demonstration, it is not necessary for 
        // the method to do anything useful. 
        //
        public static Derived MyMethod(Base arg)
        {
            Base dummy = arg;
            return new Derived();
        }
    }

    // Define a delegate that takes an instance of Derived and returns an
    // instance of Base.
    //参数类型和返回类型的兼容性
    public delegate Base Example(Derived arg);

    class Test
    {
        public static void Main()
        {
            // The binding flags needed to retrieve MyMethod.
            BindingFlags flags = BindingFlags.Public | BindingFlags.Static;

            // Get a MethodInfo that represents MyMethod.
            MethodInfo minfo = typeof(Derived).GetMethod("MyMethod", flags);

            // Demonstrate contravariance of parameter types and covariance
            // of return types by using the delegate Example to represent
            // MyMethod. The delegate binds to the method because the
            // parameter of the delegate is more restrictive than the 
            // parameter of the method (that is, the delegate accepts an
            // instance of Derived, which can always be safely passed to
            // a parameter of type Base), and the return type of MyMethod
            // is more restrictive than the return type of Example (that
            // is, the method returns an instance of Derived, which can
            // always be safely cast to type Base). 
            //
            Example ex =
                (Example)Delegate.CreateDelegate(typeof(Example), minfo);

            // Execute MyMethod using the delegate Example.
            //        
            Base b = ex(new Derived());
        }
    }
}
