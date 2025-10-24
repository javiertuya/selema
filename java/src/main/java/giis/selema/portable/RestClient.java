package giis.selema.portable;

import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStream;
import java.nio.charset.StandardCharsets;

import org.apache.hc.client5.http.classic.methods.*;
import org.apache.hc.client5.http.impl.classic.CloseableHttpClient;
import org.apache.hc.client5.http.impl.classic.HttpClients;
import org.apache.hc.core5.http.ClassicHttpResponse;
import org.apache.hc.core5.http.io.entity.EntityUtils;

public class RestClient {

	private RestClient() {
		throw new IllegalStateException("Utility class");
	}

	/**
     * Performs a REST operation to the video service.
     * 
     * @param method HTTP method: "POST", "GET", "DELETE"
     * @param url Full URL including path parameters
     * @returns the response body as string
     */
	public static void restCall(String method, String url) {
		restCall(method, url, null);
	}
	
    /**
     * Performs a REST GET operation to download the binary response body into a file.
     * 
     * @param url Full URL including path parameters
     * @param the path of the file where the response body is stored
     */
	public static void restDownload(String url, String targetFile) {
		try (FileOutputStream out = new FileOutputStream(targetFile)) {
			RestClient.restCall("GET", url, out);
		} catch (IOException e) {
			throw new VideoControllerException("Can't get video file: " + e.getMessage(), e);
		}
	}
	
    /**
     * Performs a REST operation to the video service.
     * 
     * @param method HTTP method: "POST", "GET", "DELETE"
     * @param url Full URL including path parameters
     * @param outputStream  Optional: if GET and binary response, writes to this stream
     */
	public static String restCall(String method, String url, OutputStream outputStream) {
		try (CloseableHttpClient client = HttpClients.createDefault()) {
			HttpUriRequestBase request = getRequest(method, url);

			try (ClassicHttpResponse response = client.executeOpen(null, request, null)) {
				int status = response.getCode();

				if (status >= 200 && status < 300) {
					if ("GET".equalsIgnoreCase(method) && outputStream != null) { // to download binary
						response.getEntity().writeTo(outputStream);
						return "downloaded";
					} else {
						return EntityUtils.toString(response.getEntity(), StandardCharsets.UTF_8);
					}
				} else {
					String error = EntityUtils.toString(response.getEntity(), StandardCharsets.UTF_8);
					throw new VideoControllerException("HTTP error " + status + ": " + error);
				}
			}
		} catch (Exception e) {
			throw new VideoControllerException("Unexpected http client exception, method: " 
					+ method + ", url: " + url + ", message: " + e.getMessage(), e);
		}
	}
    
	private static HttpUriRequestBase getRequest(String method, String url) {
		switch (method.toUpperCase()) {
		case "POST":
			return new HttpPost(url);
		case "GET":
			return new HttpGet(url);
		case "DELETE":
			return new HttpDelete(url);
		default:
			throw new VideoControllerException("Unsupported HTTP method: " + method);
		}
	}
}
