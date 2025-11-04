export class Log {

  static info(message) {
    console.log(`${new Date().toLocaleString()} [INFO]  ${message}`);
  }
  static debug(message) {
    console.debug(`${new Date().toLocaleString()} [DEBUG] ${message}`);
  }
  static trace(message) {
    //console.debug(`${new Date().toLocaleString()} [TRACE] ${message}`);
  }
  static error(message) {
    console.error(`${new Date().toLocaleString()} [ERROR] ${message}`);
  }
}
