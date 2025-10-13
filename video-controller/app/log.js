export class Log {

  static info(message) {
    console.log(`${new Date().toLocaleString()} [INFO]  ${message}`);
  }
  static debug(message) {
    console.debug(`${new Date().toLocaleString()} [DEBUG] ${message}`);
  }
  static error(message) {
    console.error(`${new Date().toLocaleString()} [ERROR] ${message}`);
  }
}
