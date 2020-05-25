using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace TestGeneratedFile
{
    class Program
    {
	    private static Actuator element0;
		private static AndOperation element1;
		private static Condition element2;
		private static Condition element3;        
		public static void Main(string[] args)
        {
 
			element0 = new Actuator(0, 100); 
			IObservable<int> observable0 =
				System.Reactive.Linq.Observable.FromEventPattern<int>(
				h => element0.Event += h, h => element0.Event -= h)
				.Select(e => e.EventArgs).Synchronize().DistinctUntilChanged();
			IObserver<int> observer0 = Observer.Create<int>(x => element0.Action(x));
			ISubject<int> reactElement0 = Subject.Create<int>(observer0, observable0); 
      
			element1 = new AndOperation(1); 
			IObservable<int> observable1 =
				System.Reactive.Linq.Observable.FromEventPattern<int>(
				h => element1.Event += h, h => element1.Event -= h)
				.Select(e => e.EventArgs).Synchronize().DistinctUntilChanged();
			IObserver<int> observer1 = Observer.Create<int>(x => element1.Action(x));
			ISubject<int> reactElement1 = Subject.Create<int>(observer1, observable1); 
      
			element2 = new Condition(2); 
			IObservable<int> observable2 =
				System.Reactive.Linq.Observable.FromEventPattern<int>(
				h => element2.Event += h, h => element2.Event -= h)
				.Select(e => e.EventArgs).Synchronize().DistinctUntilChanged();
			IObserver<int> observer2 = Observer.Create<int>(x => element2.Action(x));
			ISubject<int> reactElement2 = Subject.Create<int>(observer2, observable2); 
      
			element3 = new Condition(3, 5, false); 
			IObservable<int> observable3 =
				System.Reactive.Linq.Observable.FromEventPattern<int>(
				h => element3.Event += h, h => element3.Event -= h)
				.Select(e => e.EventArgs).Synchronize().DistinctUntilChanged();
			IObserver<int> observer3 = Observer.Create<int>(x => element3.Action(x));
			ISubject<int> reactElement3 = Subject.Create<int>(observer3, observable3);



			element1.IncomingValues.Add(2, null);
			element1.IncomingValues.Add(3, null);

            var sub0 = reactElement1.Subscribe(reactElement0);
            var sub1 = reactElement2.Subscribe(reactElement1);
            var sub2 = reactElement3.Subscribe(reactElement1);       
            
			SensorSim sensorSim2 = new SensorSim();
			sensorSim2.Index = 2;
            Console.WriteLine("Sensor{0} is Active", 2);

            IObservable<int> observableForSimulation2 =
						System.Reactive.Linq.Observable.FromEventPattern<int>(
							h => sensorSim2.Event += h,
							h => sensorSim2.Event -= h)
							.Select(e => e.EventArgs)
							.Synchronize().DistinctUntilChanged();

            observableForSimulation2.Subscribe(reactElement2);
            SensorSim sensorSim3 = new SensorSim();
			sensorSim3.Index = 3;
            Console.WriteLine("Sensor{0} is Active", 3);

            IObservable<int> observableForSimulation3 =
						System.Reactive.Linq.Observable.FromEventPattern<int>(
							h => sensorSim3.Event += h,
							h => sensorSim3.Event -= h)
							.Select(e => e.EventArgs)
							.Synchronize().DistinctUntilChanged();

            observableForSimulation3.Subscribe(reactElement3);

            Console.ReadLine();
        }
    }
}