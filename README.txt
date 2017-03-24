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
