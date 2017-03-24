TCPServer
Ramy Shahin
March 23rd 2017

This is a simple TCP server, handling some text commands and responding accordingly to clients.
 
Design Decisions:
- The server listens on all localhost network interfaces (in case more than one is installed).
- All client commands are in ASCII characters, so there is no need to encode them in Unicode.
- Interlocked increment is used to atomically increment counters instead of using more expensive
  synchronization primitives (e.g. a mutex)
- Server functionality is decoupled from client-specific session handling (implemented in the
  Session class)
- Each session is launched as an aynchronous task (on a threadpool). This makes the server both
  more responsive and more scalable to concurrent traffic.
- PRIME is really interesting. Primality testing is known to be computationally expensive. Because
  of that, my first insight (assuming that the number of 32-bit primes is a few thousands) was to
  store 32-bit primes in a lookup table, and just pick numbers at random from that table. However,
  with some quick research (http://primos.mat.br/primeiros_10000_primos.txt), it turned out that 
  the first 10,000 primes range from 2 to 104729. At least within that range, the ration between primes
  to non-primes is about 1:9. That said, we can just pick numbers at random, test them for primality
  until a prime number is found. This is expensive, but it seems to be working fine.