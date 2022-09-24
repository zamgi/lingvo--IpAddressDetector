using System;
using System.Collections.Concurrent;
using System.Threading;

using lingvo.core;
using _IpAddressDetector_ = lingvo.core.IpAddressDetector;

namespace IpAddressDetector.WebService
{
    /// <summary>
    /// 
    /// </summary>
	public sealed class ConcurrentFactory
	{
		private Semaphore                              _Semaphore;
        private ConcurrentStack< _IpAddressDetector_ > _Stack;

        public ConcurrentFactory( int instanceCount )
		{
            if ( instanceCount <= 0 ) throw (new ArgumentException( nameof(instanceCount) ));

            _Semaphore = new Semaphore( instanceCount, instanceCount );
            _Stack = new ConcurrentStack< _IpAddressDetector_ >();
			for ( int i = 0; i < instanceCount; i++ )
			{
                _Stack.Push( new _IpAddressDetector_() );
			}
		}

        public ip_t[] Run( string text )
		{
			_Semaphore.WaitOne();
			var worker = default(_IpAddressDetector_);
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

        private static T Pop< T >( ConcurrentStack< T > stack ) => stack.TryPop( out var t ) ? t : default;
	}
}
