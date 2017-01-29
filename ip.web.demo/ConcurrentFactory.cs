using System;
using System.Collections.Concurrent;
using System.Threading;

namespace lingvo.core
{
    /// <summary>
    /// 
    /// </summary>
	internal sealed class ConcurrentFactory
	{
		private Semaphore                            _Semaphore;
        private ConcurrentStack< IpAddressDetector > _Stack;

        public ConcurrentFactory( int instanceCount )
		{
            if ( instanceCount <= 0 ) throw (new ArgumentException("instanceCount"));

            _Semaphore = new Semaphore( instanceCount, instanceCount );
            _Stack = new ConcurrentStack< IpAddressDetector >();
			for ( int i = 0; i < instanceCount; i++ )
			{
                _Stack.Push( new IpAddressDetector() );
			}			
		}

        public ip_t[] Run( string text )
		{
			_Semaphore.WaitOne();
			var worker = default(IpAddressDetector);
			try
			{
                worker = Pop( _Stack );
                if ( worker == null )
                {
                    for ( var i = 0; ; i++ )
                    {
                        worker = Pop( _Stack );
                        if ( worker != null )
                            break;

                        Thread.Sleep( 25 ); //SpinWait.SpinUntil(

                        if ( 10000 <= i )
                            throw (new InvalidOperationException( this.GetType().Name + ": no (fusking) worker item in queue" ));
                    }
                }

                var result = worker.Run( text ).ToArray();
                return (result);
			}
			finally
			{
				if ( worker != null )
				{
					_Stack.Push( worker );
				}
				_Semaphore.Release();
			}

            throw (new InvalidOperationException( this.GetType().Name + ": nothing to return (fusking)" ));
		}

        private static T Pop< T >( ConcurrentStack< T > stack )
        {
            var t = default(T);
            if ( stack.TryPop( out t ) )
                return (t);
            return (default(T));
        }
	}
}
