package com.ldsf.app

import android.content.Context
import android.util.Base64
import java.security.KeyPair
import java.security.KeyPairGenerator

object RsaKeys {
    private const val PREFS = "ldsf_keys"
    private const val PUBLIC_KEY = "public_key_pem"

    fun getOrCreatePublicKeyPem(context: Context): String {
        val prefs = context.getSharedPreferences(PREFS, Context.MODE_PRIVATE)
        prefs.getString(PUBLIC_KEY, null)?.let { return it }

        val pair = generatePair()
        val publicPem = toPem("PUBLIC KEY", pair.public.encoded)
        prefs.edit().putString(PUBLIC_KEY, publicPem).apply()
        return publicPem
    }

    private fun generatePair(): KeyPair {
        val generator = KeyPairGenerator.getInstance("RSA")
        generator.initialize(2048)
        return generator.generateKeyPair()
    }

    private fun toPem(label: String, bytes: ByteArray): String {
        val body = Base64.encodeToString(bytes, Base64.NO_WRAP)
            .chunked(64)
            .joinToString("\n")
        return "-----BEGIN $label-----\n$body\n-----END $label-----"
    }
}

