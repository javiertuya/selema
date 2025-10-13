import { exec } from 'child_process';
import { promises as fs } from 'fs';
import path from 'path';

/**
 * Utility class for command-line and file operations.
 */
export class CommandLine {
  constructor() {
    throw new Error('Utility class');
  }

  /**
   * Runs a shell command and returns stdout and stderr as a single string.
   * @param {string} command - The shell command to execute.
   * @returns {Promise<string>}
   */
  static runCommand(command) {
    return new Promise((resolve, reject) => {
      exec(command, (error, stdout, stderr) => {
        if (error) {
          reject(new Error(`Can't execute command: ${command}\n${stderr.trim()}`));
        } else {
          resolve((stdout + stderr).trim());
        }
      });
    });
  }

  /**
   * Deletes a file if it exists.
   * @param {string} fileName - Path to the file.
   * @param {boolean} throwIfNotExists - Whether to throw if the file doesn't exist.
   */
  static async fileDelete(fileName, throwIfNotExists = false) {
    try {
      await fs.unlink(fileName);
    } catch (err) {
      if (err.code === 'ENOENT' && !throwIfNotExists) {
        return;
      }
      throw new Error(`Can't delete file: ${fileName}\n${err.message}`);
    }
  }

  /**
   * Copies a file from one location to another.
   * @param {string} fileFrom - Source file path.
   * @param {string} fileTo - Destination file path.
   */
  static async fileCopy(fileFrom, fileTo) {
    try {
      await fs.copyFile(fileFrom, fileTo);
    } catch (err) {
      throw new Error(`Can't copy ${fileFrom} to ${fileTo}\n${err.message}`);
    }
  }

  /**
   * Checks if a file exists.
   * @param {string} fileName - Path to the file.
   * @returns {Promise<boolean>}
   */
  static async fileExists(fileName) {
    try {
      await fs.access(fileName);
      return true;
    } catch {
      return false;
    }
  }

  /**
   * Checks if a path is absolute.
   * @param {string} fileName - File path.
   * @returns {boolean}
   */
  static isAbsolute(fileName) {
    return path.isAbsolute(fileName);
  }
}
